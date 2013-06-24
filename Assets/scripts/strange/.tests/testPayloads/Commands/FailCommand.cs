using System;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class FailCommand : Command
	{
		public override void Execute ()
		{
			Fail ();
		}
	}
}

