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

namespace strange.framework.api
{
	public enum BinderExceptionType
	{
		/// The binder is being used while one or more Bindings are in conflict
		CONFLICT_IN_BINDER,

		/// A runtime class resolved to null. Usually caused when a class can't be resolved.
		RUNTIME_NULL_VALUE,

		/// A runtime binding was attempted with no 'Bind'
		RUNTIME_NO_BIND,

		/// Detected an unrecognized runtime type.
		RUNTIME_TYPE_UNKNOWN,

		/// A runtime binding tried to add multiple Bind keys. The current binder accepts only a single key.
		RUNTIME_TOO_MANY_KEYS,

		/// A runtime binding tried to add multiple Bind keys. The current binder accepts only a single key.
		RUNTIME_TOO_MANY_VALUES,

		/// A runtime binding tried to add something rejected by the whitelist.
		RUNTIME_FAILED_WHITELIST_CHECK
	}
}

