using System;

namespace strange.unittests
{
	public class PolymorphicClass : ISimpleInterface, IAnotherSimpleInterface
	{
		public PolymorphicClass ()
		{
		}

		#region ISimpleInterface implementation

		public int intValue { get; set;}

		#endregion

		#region IAnotherSimpleInterface implementation

		public string stringValue { get; set;}

		#endregion
	}
}

