/**
 * @interface strange.extensions.sequencer.api.ISequencer
 * 
 * Instantiates and executes one or more SequenceCommands 
 * in a specified order, based on the input key. Each SequenceCommand
 * is only instantiated and executed after the prior one in the
 * sequence has finished.
 */

using System;
using strange.extensions.command.api;

namespace strange.extensions.sequencer.api
{
	public interface ISequencer : ICommandBinder
	{
		/// Called to break the Sequence by providing a Key
		void Stop(object key);

		/// Called by a SequenceCommand to indicate that it wants to terminate a sequence to which it belongs
		void BreakSequence (ISequenceCommand command);

		/// Release a previously retained SequenceCommand.
		/// By default, a Command is garbage collected at the end of its `Execute()` method. 
		/// But the Command can be retained for asynchronous calls.
		void ReleaseCommand(ISequenceCommand command);

		new ISequenceBinding Bind<T>();

		new ISequenceBinding Bind(object value);
	}
}

