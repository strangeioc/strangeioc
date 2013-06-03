/**
 * @interface strange.extensions.injector.api.IInjectorFactory
 * 
 * Interface for the Factory that instantiates all instances.
 * 
 * @see strange.extensions.injector.api.IInjector
 */

using System;

namespace strange.extensions.injector.api
{
	public interface IInjectorFactory
	{
		/// Request instantiation based on the provided binding
		object Get (IInjectionBinding binding);

		/// Request instantiation based on the provided binding and an array of Constructor arguments
		object Get (IInjectionBinding binding, object[] args);
	}
}

