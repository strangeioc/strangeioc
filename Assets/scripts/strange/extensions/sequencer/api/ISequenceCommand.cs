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
 * @interface strange.extensions.sequencer.api.ISequenceCommand
 * 
 * Interface for Commands run by the Sequencer.
 * 
 * A SequenceCommand is simply a Command that runs in a series
 * and can optionally call `BreakSequence()` to stop the implementation
 * of that series.
 * 
 * @see strange.extensions.command.api.ICommand
 */ 

using System;
using strange.extensions.command.api;

namespace strange.extensions.sequencer.api
{
	public interface ISequenceCommand : ICommand
	{
		/// Called by a SequenceCommand that determines its entire Sequence should terminate.
		void BreakSequence();

		// Called by the Sequencer to indicate that the Sequence has been broken.
		void Cancel();

		// The property indicating that a sequence has been broken.
		bool cancelled{ get; }

		//The ordered id of this SequenceCommand, used by the Sequencer to find the next Command.
		int sequenceId{ get; set; }

	}
}

