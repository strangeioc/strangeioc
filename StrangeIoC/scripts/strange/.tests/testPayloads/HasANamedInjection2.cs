using System;

namespace strange.unittests
{
	public class HasANamedInjection2
	{
		[Inject(SomeEnum.ONE)]
		public ClassToBeInjected injected{ get; set;}
		
		
		public HasANamedInjection2 ()
		{
		}
	}
}

