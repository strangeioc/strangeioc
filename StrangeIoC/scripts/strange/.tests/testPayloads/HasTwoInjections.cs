using System;

namespace strange.unittests
{
	public class HasTwoInjections
	{
		[Inject]
		public InjectableSuperClass injectionOne{ get; set;}

		[Inject]
		public string injectionTwo{ get; set;}


		public HasTwoInjections ()
		{
		}
	}
}

