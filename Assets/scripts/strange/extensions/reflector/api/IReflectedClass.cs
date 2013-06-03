/**
 * @interface strange.extensions.reflector.api.IReflectedClass
 * 
 * Interface for representation of a class.
 * 
 * A reflection represents the already-reflected class, complete with the preferred
 * constructor, the constructor parameters, post-constructor(s) and settable
 * values.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace strange.extensions.reflector.api
{
	public interface IReflectedClass
	{
		/// Get/set the preferred constructor
		ConstructorInfo constructor{ get; set;}

		/// Get/set the preferred constructor's list of parameters
		Type[] constructorParameters{ get; set;}

		/// Get/set any PostConstructors. This includes inherited PostConstructors.
		MethodInfo[] postConstructors{ get; set;}

		/// Get/set the list of setter injections. This includes inherited setters.
		KeyValuePair<Type, PropertyInfo>[] setters{ get; set;}
		object[] setterNames{ get; set;}

		/// For testing. Allows a unit test to assert whether the binding was
		/// generated on the current call, or on a prior one.
		bool preGenerated{ get; set;}
	}
}

