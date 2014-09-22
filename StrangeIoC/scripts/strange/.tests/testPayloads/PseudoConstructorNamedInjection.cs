using System;

namespace strange.unittests
{
	public class PseudoConstructorNamedInjection
	{
		public ClassToBeInjected first;
		public ClassToBeInjected second;

		public PseudoConstructorNamedInjection ()
		{
		}

		[PseudoConstruct]
		public void PseudoConstruct(
			[Name("First")] ClassToBeInjected first,
			[Name("Second")] ClassToBeInjected second) 
		{
			this.first = first;
			this.second = second;
		}
	}
}

