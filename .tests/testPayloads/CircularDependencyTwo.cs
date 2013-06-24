using System;

namespace strange.unittests
{
	public class CircularDependencyTwo
	{
		[Inject]
		public CircularDependencyOne depends{ get; set;}

		public CircularDependencyTwo ()
		{
		}
	}
}

