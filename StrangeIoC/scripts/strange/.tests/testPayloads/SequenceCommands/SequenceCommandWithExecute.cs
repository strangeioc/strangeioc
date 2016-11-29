using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.sequencer.impl;

namespace strange.unittests
{
	public class SequenceCommandWithoutExecute : SequenceCommand
	{
		public SequenceCommandWithoutExecute ()
		{
		}

		public override void Execute()
		{
			throw new CommandException("You must override the Execute method in every Command", CommandExceptionType.EXECUTE_OVERRIDE);
		}
	}
}

