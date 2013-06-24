using System;
using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class CommandWithInjection : Command
	{
		[Inject]
		public ISimpleInterface injected{get; set;}

		override public void Execute()
		{
			injected.intValue = 100;
		}
	}
}

