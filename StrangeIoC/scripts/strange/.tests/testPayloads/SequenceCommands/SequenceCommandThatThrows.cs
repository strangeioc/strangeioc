using strange.extensions.sequencer.impl;

namespace strange.unittests
{
	public class SequenceCommandThatThrows : SequenceCommand
	{
		public override void Execute()
		{
			throw new System.NotImplementedException();
		}
	}
}
