/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.framework.impl.Binder
 * 
 * Collection class for bindings.
 * 
 * Binders are a collection class (akin to ArrayList and Dictionary)
 * with the specific purpose of connecting lists of things that are
 * not necessarily related, but need some type of runtime association.
 * Binders are the core concept of the StrangeIoC framework, allowing
 * all the other functionality to exist and further functionality to
 * easily be created.
 * 
 * Think of each Binder as a collection of causes and effects, or actions
 * and reactions. If the Key action happens, it triggers the Value
 * action. So, for example, an Event may be the Key that triggers
 * instantiation of a particular class.
 *
 * <h2>Runtime Bindings</h2>
 * As of V1.0, Strange supports runtime bindings via JSON. This allows you to 
 * instruct Strange to create its bindings by using loaded data, for example via
 * a downloaded .json file, or a server response.
 *
 * binder.ConsumeBindings(stringOfLoadedJson);
 *
 * Below are examples for basic runtime
 * binding options for Binder. The complete set of JSON runtime bindings for all
 * officially supported binders can be found in The Big, Strange How-To:
 * (http://strangeioc.github.io/strangeioc/TheBigStrangeHowTo.html)
 *
 * <h3>Example: basic binding via JSON</h3>
 * The simplest possible binding is an array of objects. We bind "This" to "That"
 *
 * [
 * 	{
 * 		"Bind":"This",
 * 		"To":"That"
 * 	}
 * ]
 *
 * You can of course load as many bindings as you like in your array:
 *
 * [
 *	{
 *		"Bind":"This",
 *		"To":"That"
 *	},
 *	{
 *		"Bind":"Han",
 *		"To":"Leia"
 *	},
 *	{
 *		"Bind":"Greedo",
 *		"To":"Table"
 *	}
 *]
 *
 * You can name bindings as you would expect:
 *
 * [
 *	{
 *		"Bind":"Battle",
 *		"To":"Planet",
 *		"ToName":"Endor" 
 *	}
 * ]
 *
 * If you need more than a single item in a "Bind" or "To" statement, use an array.
 *
 * [
 *	{
 *		"Bind":["Luke", "Han", "Wedge", "Biggs"],
 *		"To":"Pilot"
 *	}
 * ]
 *
 * There is also an "Options" array for special behaviors required by
 * individual Binders. The core Binder supports "Weak".
 *
 * [
 *	{
 *		"Bind":"X-Wing",
 *		"To":"Ship",
 *		"Options":["Weak"]
 *	}
 * ]
 *
 * Other Binders support other Options. Here's a case from the InjectionBinder. Note
 * how Options can be either a string or an array.
 *
 * [
 *	{
 *		"Bind":"strange.unittests.ISimpleInterface",
 *		"To":"strange.unittests.SimpleInterfaceImplementer",
 *		"Options":"ToSingleton"
 *	}
 * ]
 */

using System.Collections.Generic;
using strange.framework.api;
using MiniJSON;

namespace strange.framework.impl
{
	public class Binder : IBinder
	{
		/// Dictionary of all bindings
		/// Two-layer keys. First key to individual Binding keys,
		/// then to Binding names. (This wouldn't be required if
		/// Unity supported Tuple or HashSet.)
		protected Dictionary <object, Dictionary<object, IBinding>> bindings;

		protected Dictionary <object, Dictionary<IBinding, object>> conflicts;

		protected List<object> bindingWhitelist;

		/// A handler for resolving the nature of a binding during chained commands
		public delegate void BindingResolver(IBinding binding);

		public Binder ()
		{
			bindings = new Dictionary <object, Dictionary<object, IBinding>> ();
			conflicts = new Dictionary <object, Dictionary<IBinding, object>> ();
		}

		virtual public IBinding Bind<T>()
		{
			return Bind (typeof(T));
		}

		virtual public IBinding Bind(object key)
		{
			IBinding binding;
			binding = GetRawBinding ();
			binding.Bind(key);
			return binding;
		}

		virtual public IBinding GetBinding<T>()
		{
			return GetBinding (typeof(T), null);
		}

		virtual public IBinding GetBinding(object key)
		{
			return GetBinding (key, null);
		}

		virtual public IBinding GetBinding<T>(object name)
		{
			return GetBinding (typeof(T), name);
		}

		virtual public IBinding GetBinding(object key, object name)
		{
			if (conflicts.Count > 0)
			{
				string conflictSummary = "";
				Dictionary<object, Dictionary<IBinding, object>>.KeyCollection keys = conflicts.Keys;
				foreach (object k in keys)
				{
					if (conflictSummary.Length > 0)
					{
						conflictSummary+= ", ";
					}
					conflictSummary += k.ToString ();
				}
				throw new BinderException ("Binder cannot fetch Bindings when the binder is in a conflicted state.\nConflicts: " + conflictSummary, BinderExceptionType.CONFLICT_IN_BINDER);
			}

			if(bindings.ContainsKey (key))
			{
				Dictionary<object, IBinding> dict = bindings [key];
				name = (name == null) ? BindingConst.NULLOID : name;
				if (dict.ContainsKey(name))
				{
					return dict [name];
				}
			}
			return null;
		}

		virtual public void Unbind<T>()
		{
			Unbind (typeof(T), null);
		}

		virtual public void Unbind(object key)
		{
			Unbind (key, null);
		}

		virtual public void Unbind<T>(object name)
		{
			Unbind (typeof(T), name);
		}

		virtual public void Unbind(object key, object name)
		{
			if (bindings.ContainsKey(key))
			{
				Dictionary<object, IBinding> dict = bindings[key];
				object bindingName = (name == null) ? BindingConst.NULLOID : name;
				if (dict.ContainsKey(bindingName))
				{
					dict.Remove(bindingName);
				}
			}
		}

		virtual public void Unbind(IBinding binding)
		{
			if (binding == null)
			{
				return;
			}
			Unbind (binding.key, binding.name);
		}

		virtual public void RemoveValue (IBinding binding, object value)
		{
			if (binding == null || value == null)
			{
				return;
			}
			object key = binding.key;
			Dictionary<object, IBinding> dict;
			if ((bindings.ContainsKey(key)))
			{
				dict = bindings [key];
				if (dict.ContainsKey(binding.name))
				{
					IBinding useBinding = dict [binding.name];
					useBinding.RemoveValue (value);

					//If result is empty, clean it out
					object[] values = useBinding.value as object[];
					if (values == null || values.Length == 0)
					{
						dict.Remove(useBinding.name);
					}
				}
			}
		}

		virtual public void RemoveKey (IBinding binding, object key)
		{
			if (binding == null || key == null || bindings.ContainsKey(key) == false) 
			{
				return;
			}
			Dictionary<object, IBinding> dict = bindings[key];
			if (dict.ContainsKey (binding.name)) 
			{
				IBinding useBinding = dict [binding.name];
				useBinding.RemoveKey (key);
				object[] keys = useBinding.key as object[];
				if (keys != null && keys.Length == 0)
				{
					dict.Remove(binding.name);
				}
			}
		}

		virtual public void RemoveName (IBinding binding, object name)
		{
			if (binding == null || name == null) 
			{
				return;
			}
			object key;
			if (binding.keyConstraint.Equals(BindingConstraintType.ONE))
			{
				key = binding.key;
			}
			else
			{
				object[] keys = binding.key as object[];
				key = keys [0];
			}

			Dictionary<object, IBinding> dict = bindings[key];
			if (dict.ContainsKey (name)) 
			{
				IBinding useBinding = dict [name];
				useBinding.RemoveName (name);
			}
		}

		virtual public IBinding GetRawBinding()
		{
			return new Binding (resolver);
		}

		/// The default handler for resolving bindings during chained commands
		virtual protected void resolver(IBinding binding)
		{
			object key = binding.key;
			if (binding.keyConstraint.Equals(BindingConstraintType.ONE)) {
				ResolveBinding (binding, key);
			} 
			else
			{
				object[] keys = key as object[];
				int aa = keys.Length;
				for(int a = 0; a < aa; a++)
				{
					ResolveBinding (binding, keys[a]);
				}
			}
		}

		/**
		 * This method places individual Bindings into the bindings Dictionary
		 * as part of the resolving process. Note that while some Bindings
		 * may store multiple keys, each key takes a unique position in the
		 * bindings Dictionary.
		 * 
		 * Conflicts in the course of fluent binding are expected, but GetBinding
		 * will throw an error if there are any unresolved conflicts.
		 */
		virtual public void ResolveBinding(IBinding binding, object key)
		{

			//Check for existing conflicts
			if (conflicts.ContainsKey(key))	//does the current key have any conflicts?
			{
				Dictionary<IBinding, object> inConflict = conflicts [key];
				if (inConflict.ContainsKey(binding)) //Am I on the conflict list?
				{
					object conflictName = inConflict[binding];
					if (isConflictCleared(inConflict, binding)) //Am I now out of conflict?
					{
						clearConflict (key, conflictName, inConflict); //remove all from conflict list.
					}
					else
					{
						return;	//still in conflict
					}
				}		
			}

			//Check for and assign new conflicts
			object bindingName = (binding.name == null) ? BindingConst.NULLOID : binding.name;
			Dictionary<object, IBinding> dict;
			if ((bindings.ContainsKey(key)))
			{
				dict = bindings [key];
				//Will my registration create a new conflict?
				if (dict.ContainsKey(bindingName))
				{

					//If the existing binding is not this binding, and the existing binding is not weak
					//If it IS weak, we will proceed normally and overwrite the binding in the dictionary
					IBinding existingBinding = dict[bindingName];
					//if (existingBinding != binding && !existingBinding.isWeak)
					//SDM2014-01-20: as part of cross-context implicit bindings fix, attempts by a weak binding to replace a non-weak binding are ignored instead of being 
					if (existingBinding != binding)
					{
						if (!existingBinding.isWeak && !binding.isWeak)
						{
							//register both conflictees
							registerNameConflict(key, binding, dict[bindingName]);
						    return;
						}

						if (existingBinding.isWeak && (!binding.isWeak || existingBinding.value == null || existingBinding.value is System.Type))
						{
							//SDM2014-01-20: (in relation to the cross-context implicit bindings fix)
							// 1) if the previous binding is weak and the new binding is not weak, then the new binding replaces the previous;
							// 2) but if the new binding is also weak, then it only replaces the previous weak binding if the previous binding
							// has not already been instantiated:

							//Remove the previous binding.
							dict.Remove(bindingName);
						}
					}
					
				}
			}
			else
			{
				dict = new Dictionary<object, IBinding>();
				bindings [key] = dict;
			}

			//Remove nulloid bindings
			if (dict.ContainsKey(BindingConst.NULLOID) && dict[BindingConst.NULLOID].Equals(binding) )
			{
				dict.Remove (BindingConst.NULLOID);
			}

			//Add (or override) our new binding!
			if (!dict.ContainsKey(bindingName))
			{
				dict.Add (bindingName, binding);
			}
		}

		/// <summary>
		/// For consumed bindings, provide a secure whitelist of legal bindings.
		/// </summary>
		/// <param name="list"> A List of fully-qualified classnames eligible to be consumed during dynamic runtime binding.</param>
		virtual public void WhitelistBindings(List<object> list)
		{
			bindingWhitelist = list;
		}

		/// <summary>
		/// Provide the Binder with JSON data to perform runtime binding
		/// </summary>
		/// <param name="jsonString">A json-parsable string containing the bindings.</param>
		virtual public void ConsumeBindings(string jsonString)
		{
			List<object> list = Json.Deserialize(jsonString) as List<object>;
			IBinding testBinding = GetRawBinding ();

			for (int a=0, aa=list.Count; a < aa; a++)
			{
				ConsumeItem(list[a] as Dictionary<string, object>, testBinding);
			}
		}

		/// <summary>
		/// Consumes an individual JSON element and returns the Binding that element represents 
		/// </summary>
		/// <returns>The Binding represented the provided JSON</returns>
		/// <param name="item">A Dictionary of definitions for the individual binding parameters</param>
		/// <param name="testBinding">An example binding for the current Binder. This method uses the 
		/// binding constraints of the example to raise errors if asked to parse illegally</param>
		virtual protected IBinding ConsumeItem(Dictionary<string, object> item, IBinding testBinding)
		{
			int bindConstraints = (testBinding.keyConstraint == BindingConstraintType.ONE) ? 0 : 1;
			bindConstraints |= (testBinding.valueConstraint == BindingConstraintType.ONE) ? 0 : 2;
			IBinding binding = null;
			List<object> keyList;
			List<object> valueList;

			if (item != null)
			{
				item = ConformRuntimeItem (item);
				// Check that Bind exists
				if (!item.ContainsKey ("Bind"))
				{
					throw new BinderException ("Attempted to consume a binding without a bind key.", BinderExceptionType.RUNTIME_NO_BIND);
				}
				else
				{
					keyList = conformRuntimeToList (item ["Bind"]);
				}
				// Check that key counts match the binding constraint
				if (keyList.Count > 1 && (bindConstraints & 1) == 0)
				{
					throw new BinderException ("Binder " + this.ToString () + " supports only a single binding key. A runtime binding key including " + keyList [0].ToString () + " is trying to add more.", BinderExceptionType.RUNTIME_TOO_MANY_KEYS);
				}

				if (!item.ContainsKey ("To"))
				{
					valueList = keyList;
				}
				else
				{
					valueList = conformRuntimeToList (item ["To"]);
				}
				// Check that value counts match the binding constraint
				if (valueList.Count > 1 && (bindConstraints & 2) == 0)
				{
					throw new BinderException ("Binder " + this.ToString () + " supports only a single binding value. A runtime binding value including " + valueList [0].ToString () + " is trying to add more.", BinderExceptionType.RUNTIME_TOO_MANY_VALUES);
				}

				// Check Whitelist if it exists
				if (bindingWhitelist != null)
				{
					foreach (object value in valueList)
					{
						if (bindingWhitelist.IndexOf (value) == -1)
						{
							throw new BinderException ("Value " + value.ToString () + " not found on whitelist for " + this.ToString () + ".", BinderExceptionType.RUNTIME_FAILED_WHITELIST_CHECK);
						}
					}
				}

				binding = performKeyValueBindings (keyList, valueList);

				// Optionally look for ToName
				if (item.ContainsKey ("ToName"))
				{
					binding = binding.ToName (item ["ToName"]);
				}

				// Add runtime options
				if (item.ContainsKey ("Options"))
				{
					List<object> optionsList = conformRuntimeToList (item ["Options"]);
					addRuntimeOptions (binding, optionsList);
				}
			}
			return binding;
		}

		/// <summary>
		/// Override this method in subclasses to add special-case SYNTACTICAL SUGAR for Runtime JSON bindings.
		/// For example, if your Binder needs a special JSON tag BindView, such that BindView is simply
		/// another way of expressing 'Bind', override this method conform the sugar to
		/// match the base definition (BindView becomes Bind).
		/// </summary>
		/// <returns>The conformed Dictionary.</returns>
		/// <param name="dictionary">A Dictionary representing the options for a Binding.</param>
		virtual protected Dictionary<string, object> ConformRuntimeItem(Dictionary<string, object> dictionary)
		{
			return dictionary;
		}

		/// <summary>
		/// Performs the key value bindings for a JSON runtime binding.
		/// </summary>
		/// <returns>A Binding.</returns>
		/// <param name="keyList">A list of things to Bind.</param>
		/// <param name="valueList">A list of the things to which we're binding.</param>
		virtual protected IBinding performKeyValueBindings(List<object> keyList, List<object> valueList)
		{
			IBinding binding = null;

			// Bind in order
			foreach (object key in keyList)
			{
				binding = Bind (key);
			}
			foreach (object value in valueList)
			{
				binding = binding.To (value);
			}

			return binding;
		}

		/// <summary>
		/// Override this method to decorate subclasses with further runtime capabilities.
		/// For example, InjectionBinder adds ToSingleton and CrossContext capabilities so that
		/// these can be specified in JSON.
		/// 
		/// By default, the Binder supports 'Weak' as a runtime option.
		/// </summary>
		/// <returns>The provided Binding.</returns>
		/// <param name="binding">A Binding to have capabilities added.</param>
		/// <param name="options">The list of runtime options for this Binding.</param>
		virtual protected IBinding addRuntimeOptions(IBinding binding, List<object> options)
		{
			if (options.IndexOf ("Weak") > -1)
			{
				binding.Weak();
			}
			return binding;
		}

		/// <summary>
		/// Convert the object to List<object>
		/// </summary>
		/// <returns>If a List, returns the original object, typed to List<object>. If a value, creates a List and adds the value to it.</returns>
		/// <param name="bindObject">The object on which we're operating.</param>
		protected List<object> conformRuntimeToList(object bindObject)
		{
			List<object> conformed = new List<object> ();

			string t = bindObject.GetType().ToString ();
			if (t.IndexOf ("System.Collections.Generic.List") > -1)
			{
				return bindObject as List<object>;
			}

			// Conform strings to Lists
			switch (t)
			{
				case "System.String":
					string stringValue = bindObject as string;
					conformed.Add(stringValue);
					break;
				case "System.Int64":
					int intValue = (int)bindObject;
					conformed.Add(intValue);
					break;
				case "System.Double":
					float floatValue = (float)bindObject;
					conformed.Add(floatValue);
					break;
				default:
					throw new BinderException ("Runtime binding keys (Bind) must be strings or numbers.\nBinding detected of type " + t, BinderExceptionType.RUNTIME_TYPE_UNKNOWN);
			}
			return conformed;
		}

		/// Take note of bindings that are in conflict.
		/// This occurs routinely during fluent binding, but will spark an error if
		/// GetBinding is called while this Binder still has conflicts.
		protected void registerNameConflict(object key, IBinding newBinding, IBinding existingBinding)
		{
			Dictionary<IBinding, object> dict;
			if (conflicts.ContainsKey(key) == false)
			{
				dict = new Dictionary<IBinding, object> ();
				conflicts [key] = dict;
			}
			else
			{
				dict = conflicts [key];
			}
			dict [newBinding] = newBinding.name;
			dict [existingBinding] = newBinding.name;
		}

		/// Returns true if the provided binding and the binding in the dict are no longer conflicting
		protected bool isConflictCleared(Dictionary<IBinding, object> dict, IBinding binding)
		{
			foreach (KeyValuePair<IBinding, object> kv in dict)
			{
				if (kv.Key != binding && kv.Key.name == binding.name)
				{
					return false;
				}
			}
			return true;
		}

		protected void clearConflict(object key, object name, Dictionary<IBinding, object> dict)
		{
			List<IBinding> removalList = new List<IBinding>();

			foreach(KeyValuePair<IBinding, object> kv in dict)
			{
				object v = kv.Value;
				if (v.Equals(name))
				{
					removalList.Add (kv.Key);
				}
			}
			int aa = removalList.Count;
			for (int a = 0; a < aa; a++)
			{
				dict.Remove(removalList[a]);
			}
			if (dict.Count == 0)
			{
				conflicts.Remove (key);
			}
		}

		protected T[] spliceValueAt<T>(int splicePos, object[] objectValue)
		{
			T[] newList = new T[objectValue.Length - 1];
			int mod = 0;
			int aa = objectValue.Length;
			for(int a = 0; a < aa; a++)
			{
				if (a == splicePos)
				{
					mod = -1;
					continue;
				}
				newList [a + mod] = (T)objectValue [a];
			}
			return newList;
		}

		/// Remove the item at splicePos from the list objectValue 
		protected object[] spliceValueAt(int splicePos, object[] objectValue)
		{
			return spliceValueAt<object>(splicePos, objectValue);
		}

		virtual public void OnRemove() { }
	}
}

