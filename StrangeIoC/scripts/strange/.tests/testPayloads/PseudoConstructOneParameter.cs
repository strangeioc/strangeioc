using System;

namespace strange.unittests
{
	public class PseudoConstructOneParameter
	{
		public bool PseudoConstructed { get; set; }
		public ClassToBeInjected InjectedClass { get; set; }

		public PseudoConstructOneParameter ()
		{
			PseudoConstructed = false;
		}

		[PseudoConstruct]
		public void PseudoConstruct(ClassToBeInjected injectedClass)
		{
			InjectedClass = injectedClass;
			PseudoConstructed = true;
		}
	}
}

