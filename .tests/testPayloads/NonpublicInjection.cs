using System;

namespace strange.unittests
{
	public class NonpublicInjection
	{
		[Inject]
		public InjectableSuperClass injectionOne{ get; set;}

		// Oops! Forgot to mark as public
		[Inject]
		string injectionTwo{ get; set;}


		public NonpublicInjection ()
		{
		}
	}
}

