using System;

namespace strange.unittests
{
	public class ConstructorNamedInjection
	{
		public ClassToBeInjected instance1;
		public ClassToBeInjected instance2;

		public ConstructorNamedInjection(
			[Name("First")] ClassToBeInjected instance1,
			[Name("Second")] ClassToBeInjected instance2)
		{
			this.instance1 = instance1;
			this.instance2 = instance2;
		}
	}
}

