using System;
using NUnit.Framework;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestPool
	{
		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test]
		public void TestLength ()
		{
			object[] objectValue = new object[5];
			objectValue[0] = "hipster";

			//Assert.AreEqual(4, objectValue.GetUpperBound(0));
			//Assert.AreEqual(5, objectValue.Length);
			//Assert.AreEqual(5, objectValue.GetLongLength(0));
			//Assert.AreEqual(0, objectValue.GetLowerBound(0));

		}

		[Test]
		public void TestAbacus()
		{
			Pool pool = new Pool ();

			Assert.AreEqual (0, pool.getEmptySlot());
			pool.fillSlot (pool.getEmptySlot ());
			Assert.AreEqual (int.MaxValue - 1, pool.tracker);
			Assert.AreEqual (1, pool.getEmptySlot());
			pool.fillSlot (pool.getEmptySlot ());
			Assert.AreEqual (int.MaxValue - 2, pool.tracker);

		}
	}
}

