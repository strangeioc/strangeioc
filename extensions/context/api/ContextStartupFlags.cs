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

/*
* Flags to interrupt the Context startup process.
*/

using System;

namespace strange.extensions.context.api
{
	[Flags]
	public enum ContextStartupFlags
	{
		/// Context will map bindings and launch automatically (default).
		AUTOMATIC = 0,
		/// Context startup will halt after Core bindings are mapped, but before instantiation or any custom bindings.
		/// If this flag is invoked, the developer must call context.Start()
		MANUAL_MAPPING = 1,
		/// Context startup will halt after all bindings are mapped, but before firing ContextEvent.START (or the analogous Signal).
		/// If this flag is invoked, the developer must call context.Launch()
		MANUAL_LAUNCH = 2,
	}
}

