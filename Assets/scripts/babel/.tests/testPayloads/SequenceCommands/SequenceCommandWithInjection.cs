using System;
using babel.extensions.sequencer.impl;

namespace babel.unittests
{
	public class SequenceCommandWithInjection : SequenceCommand
	{
		[Inject]
		public ISimpleInterface injected{get; set;}

		override public void Execute()
		{
			injected.intValue = 100;
		}
	}
}

