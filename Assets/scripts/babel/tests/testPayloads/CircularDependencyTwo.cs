using System;

namespace babel.unittests
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

