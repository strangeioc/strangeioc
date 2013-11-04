using System;
using strange.extensions.signal.impl;

namespace strange.unittests
{
	public class CommandWithInjectionAndSignal : CommandWithInjection
	{
		[Inject]
		public Signal<SimpleInterfaceImplementer> signal { get; set;}

		override public void Execute()
		{
			injected.intValue = 100;
			signal.Dispatch (injected as SimpleInterfaceImplementer);
		}
	}
}

