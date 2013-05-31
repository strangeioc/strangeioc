using System;

namespace strange.unittests
{
	public class ClassWithConstructorParameters : ISimpleInterface
	{
		private int _intValue;

		//Two constructors. One is tagged to be the constructor used during injection
		public ClassWithConstructorParameters ()
		{
		}

		[Construct]
		public ClassWithConstructorParameters (int intValue)
		{
			this._intValue = intValue;
		}

		[DeConstruct]
		public void DeConstruct ()
		{
			this._intValue = 0;
		}

		public int intValue
		{
			get
			{
				return _intValue;
			}
			set
			{
				_intValue = value;
			}
		}
	}
}

