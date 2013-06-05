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
using strange.framework.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.reflector.api;
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

		public object GetInstance(Type key, object name)
		{
			IInjectionBinding binding = GetBinding (key, name) as IInjectionBinding;
			if (binding == null)
			{
				throw new InjectionException ("InjectionBinder has no binding for:\n\tkey: " + key + "\nname: " + name, InjectionExceptionType.NULL_BINDING);
			}
			object instance = injector.Instantiate (binding);
			return instance;
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

		new public IInjectionBinding GetBinding<T>()
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

		new public IInjectionBinding GetBinding(object key, object name)
		{
			return base.GetBinding (key, name) as IInjectionBinding;
		}
	}
}

