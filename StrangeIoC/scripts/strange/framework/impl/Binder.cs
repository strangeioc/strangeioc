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
 */

using System;
using System.Collections.Generic;
using strange.framework.api;

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
					if (existingBinding != binding && !existingBinding.isWeak && !binding.isWeak)
					{
						//register both conflictees
						registerNameConflict(key, binding, dict[bindingName]);
						return;
					}

					if (existingBinding.isWeak)
					{
						//SDM2014-01-20: (in relation to the cross-context implicit bindings fix)
						// 1) if the previous binding is weak and the new binding is not weak, then the new binding replaces the previous;
						// 2) but if the new binding is also weak, then it only replaces the previous weak binding if the previous binding
						// has not already been instantiated:
						if (existingBinding != binding && existingBinding.isWeak && ( !binding.isWeak || existingBinding.value==null || existingBinding.value is System.Type))
						{
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
			if (dict.ContainsKey(BindingConst.NULLOID) && dict[BindingConst.NULLOID] == binding)
			{
				dict.Remove (BindingConst.NULLOID);
			}

			//Add (or override) our new binding!
			if (!dict.ContainsKey(bindingName))
			{
				dict.Add (bindingName, binding);
			}

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

