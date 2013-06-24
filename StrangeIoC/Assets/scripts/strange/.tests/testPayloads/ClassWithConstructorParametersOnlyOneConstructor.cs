using System;

namespace strange.unittests
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

