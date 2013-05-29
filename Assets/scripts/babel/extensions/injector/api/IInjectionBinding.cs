using System;
using babel.framework.api;

namespace babel.extensions.injector.api
{
	public interface IInjectionBinding
	{
		IInjectionBinding AsSingleton();

		IInjectionBinding AsValue (object o);
		
		IInjectionBinding ToInject(bool value);

		InjectionBindingType type{get; set;}
		bool toInject{get;}

		//Syntactic sugar. Bind is the same as Key, but allows the syntax: Bind<T>().Bind<T>()
		IInjectionBinding Bind<T>();
		IInjectionBinding Bind(object key);

		///////////
		/// Below this point is facade for IBinding
		IInjectionBinding Key<T>();
		IInjectionBinding Key(object key);
		IInjectionBinding To<T>();
		IInjectionBinding To(object o);
		IInjectionBinding ToName<T> ();
		IInjectionBinding ToName (object o);
		IInjectionBinding Named<T>();
		IInjectionBinding Named(object o);

		object key{ get; }
		object name{ get; }
		object value{ get; }
		Enum keyConstraint{ get; set;}
		Enum valueConstraint{ get; set;}
	}
}

