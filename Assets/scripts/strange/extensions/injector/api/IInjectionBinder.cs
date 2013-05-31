using System;
using strange.framework.api;

namespace strange.extensions.injector.api
{
	public interface IInjectionBinder
	{
		IInjector injector{ get; set;}
		object GetInstance(Type key);
		object GetInstance(Type key, object name);
		object GetInstance<T>();
		object GetInstance<T>(object name);

		/// Facade for IBinder
		IInjectionBinding Bind<T>();
		IInjectionBinding Bind(Type key);
		IInjectionBinding GetBinding<T>();
		IInjectionBinding GetBinding<T>(object name);
		IInjectionBinding GetBinding(object key);
		IInjectionBinding GetBinding(object key, object name);
		void Unbind<T>();
		void Unbind<T>(object name);
		void Unbind (object key);
		void Unbind (object key, object name);
		void Unbind (IBinding binding);
	}
}

