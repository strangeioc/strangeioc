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

