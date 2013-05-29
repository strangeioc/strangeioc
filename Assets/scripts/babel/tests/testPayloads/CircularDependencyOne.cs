using System;

namespace babel.unittests
{
	public class CircularDependencyOne
	{
		[Inject]
		public CircularDependencyTwo depends{ get; set;}

		public CircularDependencyOne ()
		{
		}
	}
}

