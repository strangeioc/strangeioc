using System;

namespace strange.unittests
{
	public class MultipleConstructorsUntagged
	{
		private int _intValue;
		private string _stringValue;
		private float _floatValue;
		private ISimpleInterface _simple;

		//Two constructors. One has three params, one has four
		public MultipleConstructorsUntagged (ISimpleInterface simple, int intValue, string stringValue)
		{
			this.simple = simple;
			this.intValue = intValue;
			this.stringValue = stringValue;
		}

		public MultipleConstructorsUntagged (ISimpleInterface simple, int intValue, string stringValue, float floatValue)
		{
			this.simple = simple;
			this.intValue = intValue;
			this.stringValue = stringValue;
			this.floatValue = floatValue;
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

		public float floatValue
		{
			get
			{
				return _floatValue;
			}
			set
			{
				_floatValue = value;
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

		public ISimpleInterface simple
		{
			get
			{
				return _simple;
			}
			set
			{
				_simple = value;
			}
		}
	}
}

