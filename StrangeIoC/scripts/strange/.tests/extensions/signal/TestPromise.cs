using NUnit.Framework;
using strange.extensions.signal.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestPromise
	{

		private int value = 0;

		private const int add = 10;
		private const int addTwo = 10;
		private const int addThree = 10;
		private const int addFour = 10;

		[SetUp]
		public void setup()
		{
			value = 0;
		}
		[Test]
		public void TestNoArgPromise()
		{
			Promise promise = new Promise();
			promise.Then(NoArgCallback);
			promise.Dispatch();
			Assert.AreEqual(1, value);
		}

		[Test]
		public void TestOneArgPromise()
		{
			Promise<int> promise = new Promise<int>();
			promise.Then(OneArgCallback);
			promise.Dispatch(add);
			Assert.AreEqual(add, value);
		}

		[Test]
		public void TestTwoArgPromise()
		{
			Promise<int, int> promise = new Promise<int, int>();
			promise.Then(TwoArgCallback);
			promise.Dispatch(add, addTwo);
			Assert.AreEqual(add+addTwo, value);
		}

		[Test]
		public void TestThreeArgPromise()
		{
			Promise<int, int, int> promise = new Promise<int, int, int>();
			promise.Then(ThreeArgCallback);
			promise.Dispatch(add,addTwo,addThree);
			Assert.AreEqual(add + addTwo + addThree, value);
		}

		[Test]
		public void TestFourArgPromise()
		{
			Promise<int, int, int, int> promise = new Promise<int, int, int, int>();
			promise.Then(FourArgCallback);
			promise.Dispatch(add, addTwo, addThree, addFour);
			Assert.AreEqual(add + addTwo + addThree + addFour, value);
		}

		[Test]
		public void TestDispatchFirst()
		{
			Promise<int> promise = new Promise<int>();
			Assert.AreEqual(0, value);
			promise.Dispatch(add);
			Assert.AreEqual(0, value);

			promise.Then(OneArgCallback);
			Assert.AreEqual(add, value);
		}

		[Test]
		public void TestDispatchFirstTwo()
		{
			Promise<int, int> promise = new Promise<int, int>();
			promise.Dispatch(add, addTwo);
			Assert.AreEqual(0, value);

			promise.Then(TwoArgCallback);
			Assert.AreEqual(add+addTwo, value);
		}

		[Test]
		public void TestRemoveListener()
		{
			Promise promise = new Promise();
			promise.Then(NoArgCallback);

			promise.RemoveListener(NoArgCallback);

			promise.Dispatch();
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveOnlyRemovesThatListener()
		{
			Promise<int> promise = new Promise<int>();
			promise.Then(OneArgCallback);
			promise.Then(OneArgCallbackTwo);
			promise.RemoveListener(OneArgCallback);

			promise.Dispatch(add);
			Assert.AreEqual(add*2, value);
		}

		[Test]
		public void TestRemoveAllListeners()
		{
			Promise<int> promise = new Promise<int>();

			promise.Then(OneArgCallback);
			promise.Then(OneArgCallbackTwo);
			promise.RemoveAllListeners();

			promise.Dispatch(add);
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestMultipleListeners()
		{
			Promise<int> promise = new Promise<int>();
			promise.Then(OneArgCallback);
			promise.Then(OneArgCallbackTwo);

			promise.Dispatch(add);
			Assert.AreEqual(add+(add * 2), value);
		}

		[Test]
		public void TestThenOrder()
		{
			Promise<int> promise = new Promise<int>();
			promise.Then(OneArgCallback);
			promise.Then(OneArgCallbackTwo);
			promise.Then(OneArgCallbackThree);

			promise.Dispatch(add);
			Assert.AreEqual((add+(add * 2))*add, value);

			value = 0;

			promise = new Promise<int>();
			promise.Then(OneArgCallback);
			promise.Then(OneArgCallbackThree);
			promise.Then(OneArgCallbackTwo);

			promise.Dispatch(add);
			Assert.AreEqual((add*add)+(add*2), value);
		}

		[Test]
		public void TestRemoveFromMiddle()
		{
			Promise<int> promise = new Promise<int>();
			promise.Then(OneArgCallback);
			promise.Then(OneArgCallbackTwo);
			promise.Then(OneArgCallbackThree);

			promise.RemoveListener(OneArgCallbackTwo);
			promise.Dispatch(add);
			Assert.AreEqual(add * add, value);
		}

		private void NoArgCallback()
		{
			value++;
		}

		private void OneArgCallback(int argOne)
		{
			value += argOne;
		}

		private void OneArgCallbackTwo(int argOne)
		{
			value += argOne * 2;
		}

		private void OneArgCallbackThree(int argOne)
		{
			value *= argOne;
		}
		private void TwoArgCallback(int argOne, int argTwo)
		{
			value += argOne;
			value += argTwo;
		}
		private void ThreeArgCallback(int argOne, int argTwo, int argThree)
		{
			value += argOne;
			value += argTwo;
			value += argThree;
		}
		private void FourArgCallback(int argOne, int argTwo, int argThree, int argFour)
		{
			value += argOne;
			value += argTwo;
			value += argThree;
			value += argFour;
		}
	}
}
