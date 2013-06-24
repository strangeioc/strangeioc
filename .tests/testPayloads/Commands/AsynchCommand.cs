using System;
using System.Timers;
using strange.extensions.command.impl;

namespace strange.unittests
{
	public class AsynchCommand : Command
	{
		[Inject]
		public Timer timer{ get; set;}

		public override void Execute ()
		{
			Retain ();
			timer.Elapsed += new ElapsedEventHandler(onTimerElapsed);
			timer.Interval = 100;
		}

		private void onTimerElapsed(object source, ElapsedEventArgs e)
		{
			throw new Exception ("That worked");
		}
	}
}

