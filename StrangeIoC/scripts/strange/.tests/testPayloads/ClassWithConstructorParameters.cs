using System;

namespace strange.unittests
{
	public class ClassWithConstructorParameters : ISimpleInterface
	{
		private int _intValue;
		private string _stringValue;

		//Two constructors. One is tagged to be the constructor used during injection
		public ClassWithConstructorParameters ()
		{
		}

		[Construct]
		public ClassWithConstructorParameters (int intValue, string stringValue)
		{
			this._intValue = intValue;
			this._stringValue = stringValue;
		}

		[Deconstruct]
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

		public string stringValue
		{
			get
			{
				return _stringValue;
			}
			set
			{
				_stringValue = value;
			}
		}
	}
}

