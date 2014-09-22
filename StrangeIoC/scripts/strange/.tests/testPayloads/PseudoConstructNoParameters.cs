using System;

namespace strange.unittests
{
	public class PseudoConstructNoParameters
	{
		public bool PseudoConstructed { get; set; }

		public PseudoConstructNoParameters ()
		{
			PseudoConstructed = false;
		}

		[PseudoConstruct]
		public void PseudoConstruct()
		{
			PseudoConstructed = true;
		}
	}
}

