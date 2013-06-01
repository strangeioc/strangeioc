/**
 * ReflectedClass value object
 * 
 * A reflection represents the already-reflected class, complete with the preferred
 * constructor, the constructor parameters, post-constructor(s) and settable
 * values.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using strange.extensions.reflector.api;

namespace strange.extensions.reflector.impl
{
	public class ReflectedClass : IReflectedClass
	{
		public ConstructorInfo constructor{ get; set;}
		public Type[] constructorParameters{ get; set;}
		public MethodInfo[] postConstructors{ get; set;}
		public KeyValuePair<Type, PropertyInfo>[] setters{ get; set;}
		public object[] setterNames{ get; set;}

		//For testing. Allows a unit test to assert whether the binding was
		//generated on the current call, or on a prior one.
		public bool preGenerated{ get; set;}
	}
}

