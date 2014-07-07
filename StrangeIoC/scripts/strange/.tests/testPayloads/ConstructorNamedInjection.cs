using System;

namespace strange.unittests
{
	public class ConstructorNamedInjection
	{
		public ClassToBeInjected first;
		public ClassToBeInjected second;

		public ConstructorNamedInjection(
			[Name("First")] ClassToBeInjected first,
			[Name("Second")] ClassToBeInjected second)
		{
			this.first = first;
			this.second = second;
		}
	}
}

