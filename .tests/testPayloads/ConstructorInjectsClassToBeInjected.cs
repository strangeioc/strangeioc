using System;
namespace strange.unittests
{
	public class ConstructorInjectsClassToBeInjected
	{
		public ClassToBeInjected injected;
		public ConstructorInjectsClassToBeInjected (ClassToBeInjected injected)
		{
			this.injected = injected;
		}
	}
}

