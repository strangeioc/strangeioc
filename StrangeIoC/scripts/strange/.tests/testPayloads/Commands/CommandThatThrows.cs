using strange.extensions.command.impl;

namespace strange.unittests
{
	public class CommandThatThrows : Command
	{
		public override void Execute()
		{
			throw new System.NotImplementedException();
		}
	}
}
