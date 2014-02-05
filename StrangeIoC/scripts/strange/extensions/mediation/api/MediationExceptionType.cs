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

using System;

namespace strange.extensions.mediation.api
{
	public enum MediationExceptionType
	{
		/// Exception raised when a View can't locate a Context.
		/// Views contact the Context by "bubbling" their existence up 
		/// the display chain (recursively using transform.parent).
		/// If a View reaches the top of that chain without locating
		/// a Context, it will raise this Exception to warn you.
		/// 
		/// Note: to avoid infinite looping, there is a bubbling limit of 100
		/// layers. If your View needs to be more than 100 transforms deep, 
		/// that might signal a design problem.
		NO_CONTEXT,

		/// Exception raised when a View is mapped to itself.
		/// If a View is accidentally mapped to itself, the result will be an
		/// infinite loop of Mediation creation.
		MEDIATOR_VIEW_STACK_OVERFLOW,

		/// Exception raised when AddComponent results in a null Mediator.
		/// This probably means that the mapped "mediator" wasn't a MonoBehaviour.
		NULL_MEDIATOR,

		/// The mediator type is null on the attribute tag
		IMPLICIT_BINDING_MEDIATOR_TYPE_IS_NULL,
		/// The view type is null on the attribute tag
		IMPLICIT_BINDING_VIEW_TYPE_IS_NULL,

		/// View bound to abstraction that View doesn't actually extend/implement
		VIEW_NOT_ASSIGNABLE,
	}
}

