using System;

namespace strange.unittests
{
	public class PostConstructSeveralOrdered
	{
		public string stringVal{ get; set;}

		public PostConstructSeveralOrdered ()
		{
		}

		[PostConstruct(1)]
		public void FirstPC()
		{
			stringVal += "Z";
		}

		[PostConstruct(6)]
		public void SixthPC()
		{
			stringVal += "D";
		}

		[PostConstruct(3)]
		public void ThirdPC()
		{
			stringVal += "P";
		}

		[PostConstruct(4)]
		public void FourthPC()
		{
			stringVal += "H";
		}

		[PostConstruct(2)]
		public void SecondPC()
		{
			stringVal += "A";
		}

		[PostConstruct(5)]
		public void FifthPC()
		{
			stringVal += "O";
		}
	}
}

