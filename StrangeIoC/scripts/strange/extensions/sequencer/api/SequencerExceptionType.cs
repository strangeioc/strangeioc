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

namespace strange.extensions.sequencer.api
{
	public enum SequencerExceptionType
	{
		/// SequenceCommands must always override the Execute() method.
		EXECUTE_OVERRIDE,

		/// This exception is raised if the mapped Command doesn't implement ISequenceCommand. 
		COMMAND_USED_IN_SEQUENCE
	}
}

