/**
 * @class strange.extensions.reflector.impl.ReflectedClass
 * 
 * A reflection of a class.
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

		public bool preGenerated{ get; set;}
	}
}

