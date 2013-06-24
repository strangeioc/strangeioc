using System;

namespace strange.unittests
{
	public class HasNamedInjections
	{
		[Inject(SomeEnum.ONE)]
		public InjectableSuperClass injectionOne{ get; set;}
		
		[Inject(typeof(MarkerClass))]
		public InjectableSuperClass injectionTwo{ get; set;}


		public HasNamedInjections ()
		{
		}
	}
}

