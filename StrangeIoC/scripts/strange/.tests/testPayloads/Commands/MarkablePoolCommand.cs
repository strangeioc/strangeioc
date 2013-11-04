using System;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class MarkablePoolCommand : Command
	{
		public static int incrementValue = 0;


		public override void Execute ()
		{
			incrementValue++;
		}
	}
}


