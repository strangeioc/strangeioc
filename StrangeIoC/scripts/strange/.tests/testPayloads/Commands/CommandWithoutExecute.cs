using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class CommandWithoutExecute : Command
	{
		public CommandWithoutExecute ()
		{
		}

		public override void Execute()
		{
			throw new CommandException("You must override the Execute method in every Command", CommandExceptionType.EXECUTE_OVERRIDE);
		}
	}
}
