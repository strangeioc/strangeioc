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
 * @interface strange.extensions.context.api.ICrossContextCapable
 * 
 * API for allowing Contexts to register across the Context border.
 * 
 * Implement this interface to create a binding context that can communicate across Context boundaries.
 * 
 * MVCSContext inherits CrossContext to obtain important capabilities, such as shared injections.
 * 
 * @see strange.extensions.injector.api.IInjectionBinding
 */

using strange.extensions.dispatcher.api;
using strange.extensions.injector.api;

namespace strange.extensions.context.api
{
	public interface ICrossContextCapable
	{
		/// Add cross-context functionality to a child context being added
		void AssignCrossContext(ICrossContextCapable childContext);

		/// Clean up cross-context functionality from a child context being removed
		void RemoveCrossContext(ICrossContextCapable childContext);

		/// Request a component from the context (might be useful in certain cross-context situations)
		/// This is technically a deprecated methodology. Bind using CrossContext() instead.
		object GetComponent<T>();

		/// Request a component from the context (might be useful in certain cross-context situations)
		/// This is technically a deprecated methodology. Bind using CrossContext() instead.
		object GetComponent<T>(object name);

		/// All cross-context capable contexts must implement an injectionBinder
		ICrossContextInjectionBinder injectionBinder { get; set; }

		/// Set and get the shared system bus for communicating across contexts
		IDispatcher crossContextDispatcher { get; set; }

	}
}
