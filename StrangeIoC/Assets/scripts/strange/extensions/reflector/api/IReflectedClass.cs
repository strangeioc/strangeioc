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

