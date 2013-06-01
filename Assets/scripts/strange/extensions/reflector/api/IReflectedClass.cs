/**
 * Interface for a ReflectedClass
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
		ConstructorInfo constructor{ get; set;}
		Type[] constructorParameters{ get; set;}
		MethodInfo[] postConstructors{ get; set;}
		KeyValuePair<Type, PropertyInfo>[] setters{ get; set;}
		object[] setterNames{ get; set;}

		//For testing. Allows a unit test to assert whether the binding was
		//generated on the current call, or on a prior one.
		bool preGenerated{ get; set;}
	}
}

