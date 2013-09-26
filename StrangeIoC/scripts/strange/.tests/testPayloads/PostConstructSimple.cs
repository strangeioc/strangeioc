using System;

namespace strange.unittests
{
	public class PostConstructSimple
	{
		public static int PostConstructCount{ get; set;}

		public PostConstructSimple ()
		{
		}

		[PostConstruct]
		public void MultiplyBy2()
		{
			PostConstructCount ++;
		}
	}
}

