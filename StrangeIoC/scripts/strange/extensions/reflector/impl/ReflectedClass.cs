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
 * @class strange.extensions.reflector.impl.ReflectedClass
 * 
 * A reflection of a class.
 * 
 * A reflection represents the already-reflected class, complete with the preferred
 * constructor, the constructor parameters, post-constructor(s) and settable
 * values.
 */

using System.Collections.Generic;
using System.Linq;
using strange.extensions.reflector.api;
using System;
using System.Reflection;

namespace strange.extensions.reflector.impl
{
	public class ReflectedClass : IReflectedClass
	{
		public ConstructorInfo Constructor{ get; set;}
		public Type[] ConstructorParameters{ get; set;}
		public object[] ConstructorParameterNames { get; set; }
		public MethodInfo[] PostConstructors{ get; set;}
		public ReflectedAttribute[] Setters { get; set; }
		public object[] SetterNames{ get; set;}
		public bool PreGenerated{ get; set;}


		/// [Obsolete"Strange migration to conform to C# guidelines. Removing camelCased publics"]
		public ConstructorInfo constructor{ get { return Constructor; } set { Constructor = value; }}
		public Type[] constructorParameters{ get { return ConstructorParameters; } set { ConstructorParameters = value; }}
		public MethodInfo[] postConstructors{ get { return PostConstructors; } set { PostConstructors = value; }}
		public KeyValuePair<MethodInfo, Attribute>[] attrMethods { get; set; }
		public bool preGenerated{ get { return PreGenerated; } set { PreGenerated = value; }}


		public bool hasSetterFor(Type type)
		{
		    return Setters.Any(attr => attr.type == type);
		}
	}
}

