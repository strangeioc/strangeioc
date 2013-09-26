using System;

namespace strange.unittests
{
	public class InjectsPostConstructSimple
	{
		[Inject]
		public PostConstructSimple pcs{get;set;}

		public InjectsPostConstructSimple ()
		{
		}
	}
}

