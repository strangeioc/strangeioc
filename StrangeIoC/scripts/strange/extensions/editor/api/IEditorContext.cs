/*
 * Copyright 2015 StrangeIoC
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
 * @interface strange.extensions.context.api.IEditorContext
 * 
 * Additions for the Context unique to the Editor space.
 */


using System;
using strange.extensions.mediation.api;

namespace strange.extensions.context.api
{
	public interface IEditorContext
	{
		/// <summary>
		/// Determines whether this Context has mapping for view the specified view.
		/// </summary>
		/// <returns><c>true</c> if this Context has mapping for view the specified view; otherwise, <c>false</c>.</returns>
		/// <param name="view">An IView for use with an Editor</param>
		bool HasMappingForView(IView view);

		/// <summary>
		/// Registers the View with the Context.
		/// </summary>
		/// <param name="view">An IView for use with an Editor</param>
		void RegisterView(IView view);
	}
}


