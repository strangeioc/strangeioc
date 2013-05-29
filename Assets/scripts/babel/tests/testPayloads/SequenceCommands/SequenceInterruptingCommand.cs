using System;
using babel.extensions.sequencer.impl;

namespace babel.unittests
{
	public class SequenceInterruptingCommand : SequenceCommand
	{
		public override void Execute ()
		{
			BreakSequence ();
		}
	}
}

