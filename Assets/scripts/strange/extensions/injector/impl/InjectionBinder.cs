using System;
using strange.framework.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
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
		}

		public object GetInstance(Type key)
		{
			return GetInstance(key, null);
		}

		public object GetInstance(Type key, object name)
		{
			IInjectionBinding binding = GetBinding (key, name) as IInjectionBinding;
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

