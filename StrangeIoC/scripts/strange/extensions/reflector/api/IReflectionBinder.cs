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

