using System;
using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class CommandWithInjectionPlusSignalPayload : Command
	{
		[Inject]
		public ISimpleInterface injected{get; set;}

		[Inject]
		public int intValue { get; set; }

		override public void Execute()
		{
			injected.intValue = 100;
		}
	}
}

