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

