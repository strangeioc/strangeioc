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
		/// List of all bindings
		protected Dictionary <object, Dictionary<object, IBinding>> bindings;

		/// A handler for resolving the nature of a binding during chained commands
		public delegate void BindingResolver(IBinding binding);

		public Binder ()
		{
			bindings = new Dictionary <object, Dictionary<object, IBinding>> ();
		}

		virtual public IBinding Bind<T>()
		{
			return Bind (typeof(T));
		}

		virtual public IBinding Bind(object key)
		{
			IBinding binding;
			binding = GetRawBinding ();
			binding.Key(key);
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
				Dictionary<object, IBinding> dict = bindings [key];
				object bindingName = (name == null) ? BindingConst.NULLOID : name;
				if (dict.ContainsKey(bindingName))
				{
					dict.Remove (bindingName);
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
				resolveBinding (binding, key);
			} 
			else
			{
				object[] keys = key as object[];
				int aa = keys.Length;
				for(int a = 0; a < aa; a++)
				{
					resolveBinding (binding, keys[a]);
				}
			}
		}

		/**
		 * This method places individual Bindings into the bindings Dictionary
		 * as part of the resolving process. Note that while Bindings
		 * may store multiple keys, each key takes a unique position in the
		 * bindings Dictionary.
		 */
		virtual protected void resolveBinding(IBinding binding, object key)
		{
			Dictionary<object, IBinding> dict;
			if ((bindings.ContainsKey(key)))
			{
				dict = bindings [key];
			}
			else
			{
				dict = new Dictionary<object, IBinding>();
			}
			object bindingName = (binding.name == null) ? BindingConst.NULLOID : binding.name;
			if (dict.ContainsKey(bindingName) == false)
			{
				dict.Add (bindingName, binding);
			}
			bindings [key] = dict;
		}

		/// Remove the item at splicePos from the list objectValue 
		protected object[] spliceValueAt(int splicePos, object[] objectValue)
		{
			object[] newList = new object[objectValue.Length - 1];
			int mod = 0;
			int aa = objectValue.Length;
			for(int a = 0; a < aa; a++)
			{
				if (a == splicePos)
				{
					mod = -1;
					continue;
				}
				newList [a + mod] = objectValue [a];
			}
			return newList;
		}
	}
}

