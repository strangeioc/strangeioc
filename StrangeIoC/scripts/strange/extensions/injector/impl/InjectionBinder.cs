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
 * @class strange.extensions.injector.impl.InjectionBinder
 * 
 * The Binder for creating Injection mappings.
 * 
 * @see strange.extensions.injector.api.IInjectionBinder
 * @see strange.extensions.injector.api.IInjectionBinding
 */

using System;
using System.Collections.Generic;
using System.Linq;
using strange.framework.api;
using strange.extensions.injector.api;
using strange.extensions.reflector.impl;
using strange.framework.impl;

namespace strange.extensions.injector.impl
{
	public class InjectionBinder : Binder, IInjectionBinder
	{
		private IInjector _injector;
		protected Dictionary<Type, Dictionary<Type, IInjectionBinding>> suppliers = new Dictionary<Type, Dictionary<Type, IInjectionBinding>>();

		public InjectionBinder ()
		{
			injector = new Injector ();
			injector.binder = this;
			injector.reflector = new ReflectionBinder();
		}

		public object GetInstance(Type key)
		{
			return GetInstance(key, null);
		}

		public virtual object GetInstance(Type key, object name)
		{
			IInjectionBinding binding = GetBinding (key, name) as IInjectionBinding;
			if (binding == null)
			{
				throw new InjectionException ("InjectionBinder has no binding for:\n\tkey: " + key + "\nname: " + name, InjectionExceptionType.NULL_BINDING);
			}
			object instance = GetInjectorForBinding(binding).Instantiate (binding, false);
			injector.TryInject(binding,instance);

			return instance;
		}

		protected virtual IInjector GetInjectorForBinding(IInjectionBinding binding)
		{
			return injector;
		}

		public T GetInstance<T>()
		{
			object instance = GetInstance (typeof (T));
			T retv = (T) instance;
			return retv;
		}

		public T GetInstance<T>(object name)
		{
			object instance = GetInstance (typeof (T), name);
			T retv = (T) instance;
			return retv;
		}

		override public IBinding GetRawBinding()
		{
			return new InjectionBinding (resolver);
		}

		public IInjector injector
		{ 
			get
			{
				return _injector;
			}
			set
			{
				if (_injector != null)
				{
					_injector.binder = null;
				}
				_injector = value;
				_injector.binder = this;
			}
		}

		new public IInjectionBinding Bind<T>()
		{
			return base.Bind<T> () as IInjectionBinding;
		}

		public IInjectionBinding Bind(Type key)
		{
			return base.Bind(key) as IInjectionBinding;
		}

		new virtual public IInjectionBinding GetBinding<T>()
		{
			return base.GetBinding<T> () as IInjectionBinding;
		}

		new virtual public IInjectionBinding GetBinding<T>(object name)
		{
			return base.GetBinding<T> (name) as IInjectionBinding;
		}

		new virtual public IInjectionBinding GetBinding(object key)
		{
			return base.GetBinding (key) as IInjectionBinding;
		}

		new virtual public IInjectionBinding GetBinding(object key, object name)
		{
			return base.GetBinding(key, name) as IInjectionBinding;
		}

		public int ReflectAll()
		{
			List<Type> list = new List<Type> ();
			foreach (KeyValuePair<object, Dictionary<object, IBinding>> pair in bindings)
			{
				Dictionary<object, IBinding> dict = pair.Value;
				foreach(KeyValuePair<object, IBinding> bPair in dict)
				{
					IBinding binding = bPair.Value as IBinding;
					Type t = (binding.value is Type) ? (Type) binding.value : binding.value.GetType();
					if (list.IndexOf(t) == -1)
					{
						list.Add (t);
					}
				}
			}
			return Reflect (list);
		}

		public int Reflect(List<Type> list)
		{
			int count = 0;
			foreach(Type t in list)
			{
				//Reflector won't permit primitive types, so screen them
				if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(string))
				{
					continue;
				}
				count ++;
				injector.reflector.Get (t);
			}
			return count;
		}

		override protected IBinding performKeyValueBindings(List<object> keyList, List<object> valueList)
		{
			IBinding binding = null;

			// Bind in order
			foreach (object key in keyList)
			{
				Type keyType = Type.GetType (key as string);
				if (keyType == null)
				{
					throw new BinderException ("A runtime Injection Binding has resolved to null. Did you forget to register its fully-qualified name?\n Key:" + key, BinderExceptionType.RUNTIME_NULL_VALUE);
				}
				if (binding == null)
				{
					binding = Bind (keyType);
				}
				else
				{
					binding = binding.Bind (keyType);
				}
			}
			foreach (object value in valueList)
			{
				Type valueType = Type.GetType (value as string);
				if (valueType == null)
				{
					throw new BinderException ("A runtime Injection Binding has resolved to null. Did you forget to register its fully-qualified name?\n Value:" + value, BinderExceptionType.RUNTIME_NULL_VALUE);
				}
				binding = binding.To (valueType);
			}

			return binding;
		}

		/// Additional options: ToSingleton, CrossContext
		override protected IBinding addRuntimeOptions(IBinding b, List<object> options)
		{
			base.addRuntimeOptions (b, options);
			IInjectionBinding binding = b as IInjectionBinding;
			if (options.IndexOf ("ToSingleton") > -1)
			{
				binding.ToSingleton ();
			}
			if (options.IndexOf ("CrossContext") > -1)
			{
				binding.CrossContext ();
			}
			IEnumerable<Dictionary<string, object>> dict = options.OfType<Dictionary<string, object>> ();
			if (dict.Any())
			{
				Dictionary<string, object> supplyToDict = dict.First (a => a.Keys.Contains ("SupplyTo"));
				if (supplyToDict != null)
				{
					foreach (KeyValuePair<string,object> kv in supplyToDict)
					{
						if (kv.Value is string)
						{
							Type valueType = Type.GetType (kv.Value as string);
							binding.SupplyTo (valueType);
						}
						else
						{
							List<object> values = kv.Value as List<object>;
							for (int a = 0, aa = values.Count; a < aa; a++)
							{
								Type valueType = Type.GetType (values[a] as string);
								binding.SupplyTo (valueType);
							}
						}
					}
				}
			}

			return binding;
		}

		public IInjectionBinding GetSupplier(Type injectionType, Type targetType)
		{
			if (suppliers.ContainsKey(targetType))
			{
				if (suppliers [targetType].ContainsKey(injectionType))
				{
					return suppliers [targetType][injectionType];
				}
			}
			return null;
		}
		
		public void Unsupply(Type injectionType, Type targetType)
		{
			IInjectionBinding binding = GetSupplier(injectionType, targetType);
			if (binding != null)
			{
				suppliers [targetType].Remove(injectionType);
				binding.Unsupply(targetType);
			}
		}
		
		public void Unsupply<T, U>()
		{
			Unsupply(typeof(T), typeof(U));
		}

		override protected void resolver(IBinding binding)
		{
			IInjectionBinding iBinding = binding as IInjectionBinding;
			object [] supply = iBinding.GetSupply ();

			if (supply != null)
			{
				foreach (object a in supply)
				{
					Type aType = a as Type;
					if (suppliers.ContainsKey(aType) == false)
					{
						suppliers[aType] = new Dictionary<Type, IInjectionBinding>();
					}
					object[] keys = iBinding.key as object[];
					foreach (object key in keys)
					{
						Type keyType = key as Type;
						if (suppliers[aType].ContainsKey(keyType as Type) == false)
						{
							suppliers[aType][keyType] = iBinding;
						}
					}
				}
			}

			base.resolver (binding);
		}
	}
}

