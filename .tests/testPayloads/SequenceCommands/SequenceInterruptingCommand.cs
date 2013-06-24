using System;
using strange.extensions.sequencer.impl;

namespace strange.unittests
{
	public class SequenceInterruptingCommand : SequenceCommand
	{
		public override void Execute ()
		{
			Fail ();
		}
	}
}

