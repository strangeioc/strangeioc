/**
 * @interface strange.extensions.reflector.api.IReflectionBinder
 * 
 * Generates `ReflectedClass` instances.
 * 
 * Reflection is a slow process. This binder isolates the calls to System.Reflector 
 * and caches the result, meaning that Reflection is performed only once per class.
 * 
 * IReflectorBinder does not expose the usual Binder interface.
 * It allows only the input of a class and the output of that class's reflection.
 */

using System;
using strange.framework.api;

namespace strange.extensions.reflector.api
{
	public interface IReflectionBinder
	{
		/// Get a binding based on the provided Type
		IReflectedClass Get (Type type);

		/// Get a binding based on the provided Type generic.
		IReflectedClass Get<T> ();
	}
}

