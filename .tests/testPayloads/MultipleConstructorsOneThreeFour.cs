using System;

namespace strange.unittests
{
	public class MultipleConstructorsOneThreeFour
	{
		public int intValue;
		public string stringValue;
		public ISimpleInterface simple;

		public MultipleConstructorsOneThreeFour ()
		{
			throw new Exception ("The empty constructor in MultipleConstructorsOneThreeFour shouldn't be called.");
		}

		[Construct]
		public MultipleConstructorsOneThreeFour (ISimpleInterface simple, int intValue, string stringValue)
		{
			this.simple = simple;
			this.intValue = intValue;
			this.stringValue = stringValue;
		}
	}
}

