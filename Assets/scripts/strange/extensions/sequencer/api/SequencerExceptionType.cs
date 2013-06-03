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

