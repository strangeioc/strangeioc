using System;

namespace babel.unittests
{
	public class ClassWithConstructorParametersOnlyOneConstructor
	{
		private string _stringVal;
		public string stringVal
		{
			get
			{
				return _stringVal;
			}
		}

		public ClassWithConstructorParametersOnlyOneConstructor (string stringVal)
		{
			_stringVal = stringVal;
		}
	}
}

