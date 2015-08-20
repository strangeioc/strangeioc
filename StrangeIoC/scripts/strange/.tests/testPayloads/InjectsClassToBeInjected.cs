using System;

namespace strange.unittests
{
	public class InjectsClassToBeInjected
	{
		[Inject]
		public ClassToBeInjected injected{ get; set; }

		public InjectsClassToBeInjected ()
		{
		}
	}
}

