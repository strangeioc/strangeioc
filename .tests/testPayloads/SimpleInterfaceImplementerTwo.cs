using System;

namespace strange.unittests
{
	public class SimpleInterfaceImplementerTwo : ISimpleInterface
	{
		[Inject]
		public int intValue{ get; set;}

		public SimpleInterfaceImplementerTwo ()
		{
		}
	}
}

