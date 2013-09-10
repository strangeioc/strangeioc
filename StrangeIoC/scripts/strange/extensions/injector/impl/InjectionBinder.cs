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
using strange.framework.api;
using strange.extensions.injector.api;
using strange.extensions.reflector.impl;
using strange.framework.impl;

namespace strange.extensions.injector.impl
{
	public class InjectionBinder : Binder, IInjectionBinder
	{
		private IInjector _injector;

		public InjectionBinder ()
		{
			injector = new Injector ();
			injector.binder = this;
			injector.reflector = new ReflectionBinder ();
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
			object instance = GetInjectorForBinding(binding).Instantiate (binding);
			return instance;
		}

        protected virtual IInjector GetInjectorForBinding(IInjectionBinding binding)
        {
            return injector;
        }

		public object GetInstance<T>()
		{
			return GetInstance (typeof(T));
		}

		public object GetInstance<T>(object name)
		{
			return GetInstance (typeof(T), name);
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

		new public IInjectionBinding GetBinding<T>(object name)
		{
			return base.GetBinding<T> (name) as IInjectionBinding;
		}

		new public IInjectionBinding GetBinding(object key)
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
    }
}

