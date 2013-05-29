using System;
using babel.extensions.command.api;
using babel.extensions.command.impl;

namespace babel.unittests
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

