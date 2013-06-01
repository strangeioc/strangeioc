using System;

namespace strange.unittests
{
	public class PostConstructTwo
	{
		[Inject]
		public float floatVal{ get; set;}

		public PostConstructTwo ()
		{
		}

		[PostConstruct]
		public void MultiplyBy2()
		{
			floatVal *= 2f;
		}

		[PostConstruct]
		public void MultiplyBy2Again()
		{
			floatVal *= 2f;
		}
	}
}

