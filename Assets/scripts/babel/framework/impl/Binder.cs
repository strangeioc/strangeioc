using System;
using System.Collections.Generic;
using babel.framework.api;

namespace babel.framework.impl
{
	public class Binder : IBinder
	{
		//List of all bindings
		protected Dictionary <object, Dictionary<object, IBinding>> bindings;
		public delegate void BindingResolver(IBinding binding);

		public Binder ()
		{
			bindings = new Dictionary <object, Dictionary<object, IBinding>> ();
		}

		//Bind a Type
		virtual public IBinding Bind<T>()
		{
			return Bind (typeof(T));
		}
		
		//Bind a value
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

		//Get a binding based on the provided key Type and object name
		virtual public IBinding GetBinding<T>(object name)
		{
			return GetBinding (typeof(T), name);
		}

		//Get a binding based on the provided object and object key
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

