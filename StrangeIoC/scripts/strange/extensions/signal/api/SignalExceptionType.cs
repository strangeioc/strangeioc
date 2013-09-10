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
 * @class strange.extensions.signal.api.SignalExceptionType
 */
namespace strange.extensions.signal.api
{
	public enum SignalExceptionType
	{

		/// Attempting to bind more than one value of the same type to a command
		COMMAND_VALUE_CONFLICT,

		/// A Signal mapped to a Command found no matching injectable Type to bind a parameter to.
		COMMAND_VALUE_NOT_FOUND,

		/// SignalCommandBinder attempted to bind a null value from a signal to a Command
		COMMAND_NULL_INJECTION,
	}
}
