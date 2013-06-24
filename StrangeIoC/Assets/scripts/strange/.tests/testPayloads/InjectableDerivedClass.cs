using System;
using System.Diagnostics;

namespace strange.unittests
{
	public class InjectableDerivedClass : InjectableSuperClass
	{
		[Inject]
		public ClassToBeInjected injected{get;set;}



		public InjectableDerivedClass ()
		{
		}
		
		[PostConstruct]
		public void postConstruct1()
		{
			Console.Write ("Calling post construct 1\n");
		}
		
		[PostConstruct]
		public void postConstruct2()
		{
			Console.Write ("Calling post construct 2");
		}

		public void notAPostConstruct()
		{
			Console.Write ("notAPostConstruct :: SHOULD NOT CALL THIS!");
		}
	}
}

