using System;

namespace strange.unittests
{
	public class GuaranteedUniqueInstances
	{
		public int uid{ get; set;}

		private static int counter = 0;

		public GuaranteedUniqueInstances ()
		{
			uid = ++counter;
		}
	}
}

