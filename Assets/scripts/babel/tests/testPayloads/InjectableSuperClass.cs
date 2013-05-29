using System;

namespace babel.unittests
{
	public class InjectableSuperClass
	{
		public float floatValue{get;set;}

		[Inject]
		public int intValue{get;set;}

		public InjectableSuperClass ()
		{
		}
	}
}

