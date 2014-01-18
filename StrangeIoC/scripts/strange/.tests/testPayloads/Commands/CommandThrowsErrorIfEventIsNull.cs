using System;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class CommandThrowsErrorIfEventIsNull : EventCommand
	{
		public static int timesExecuted = 0;

		public override void Execute ()
		{
			if (evt == null)
			{
				throw new Exception ("CommandThrowsErrorIfEventIsNull had a null event");
			}
			timesExecuted++;
			System.Console.WriteLine ("Execute: " + timesExecuted);
		}
	}
}

