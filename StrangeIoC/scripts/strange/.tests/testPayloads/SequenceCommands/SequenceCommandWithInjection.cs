using System;
using strange.extensions.sequencer.impl;

namespace strange.unittests
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

