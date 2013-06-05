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

namespace strange.extensions.context.api
{
	public enum ContextKeys
	{
		/// Marker for the named Injection of the Context
		CONTEXT,
		/// Marker for the named Injection of the ContextView
		CONTEXT_VIEW,
		/// Marker for the named Injection of the contextDispatcher
		CONTEXT_DISPATCHER,
		/// Marker for the named Injection of the crossContextDispatcher
		CROSS_CONTEXT_DISPATCHER
	}
}

