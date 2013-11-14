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
 * @interface strange.framework.api.IInstanceProvider
 *
 * Provides an instance of the specified Type
 * When all you need is a new instance, use this instead of IInjectionBinder.
 */

using System;

namespace strange.framework.api
{
	public interface IInstanceProvider
	{
		/// Retrieve an Instance based on the key.
		/// ex. `injectionBinder.Get<cISomeInterface>();`
		T GetInstance<T>();

		/// Retrieve an Instance based on the key.
		/// ex. `injectionBinder.Get(typeof(ISomeInterface));`
		object GetInstance(Type key);
	}
}

