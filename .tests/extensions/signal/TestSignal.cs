using NUnit.Framework;
using strange.extensions.signal.impl;

namespace strange.unittests
{
    [TestFixture()]
	public class TestSignal
	{

        [SetUp]
        public void SetUp()
        {
            testValue = testConst;
        }

        private int testValue = 0;
        private int testInt = 5;
        private const int testConst = 42;
        [Test]
        public void TestNoArgSignal()
        {
            Signal signal = new Signal();

            Assert.AreEqual(testConst, testValue);
            signal.AddListener(NoArgSignalCallback);
            signal.Dispatch();
            Assert.AreEqual(testConst+1, testValue);
        }
        [Test]
        public void TestOneArgSignal()
        {
            Signal<int> signal = new Signal<int>();

            Assert.AreEqual(testConst, testValue);
            signal.AddListener(OneArgSignalCallback);
            signal.Dispatch(testInt);
            Assert.AreEqual(testInt + testConst, testValue);
        }

        [Test]
        public void TestOnce()
        {
            Signal<int> signal = new Signal<int>();

            Assert.AreEqual(testConst, testValue);
            signal.AddOnce(OneArgSignalCallback);
            signal.Dispatch(testInt);
            Assert.AreEqual(testInt + testConst, testValue);

            signal.Dispatch(testInt);
            Assert.AreEqual(testInt + testConst, testValue); //should not fire a second time

        }

        [Test]
        public void TestMultipleCallbacks()
        {
            Signal<int> signal = new Signal<int>();

            Assert.AreEqual(testConst, testValue);
            signal.AddListener(OneArgSignalCallback);
            signal.AddListener(OneArgSignalCallbackTwo);

            signal.Dispatch(testInt);
            Assert.AreEqual(testInt + testConst + 1, testValue);
        }

        [Test]
        public void TestRemoveListener()
        {
            Signal<int> signal = new Signal<int>();

            Assert.AreEqual(testConst, testValue);
            signal.AddListener(OneArgSignalCallback);

            signal.Dispatch(testInt);
            Assert.AreEqual(testInt + testConst, testValue);

            signal.RemoveListener(OneArgSignalCallback);
            signal.Dispatch(testInt);
            Assert.AreEqual(testInt + testConst, testValue); //should not have changed
        }

        //callbacks
        private void NoArgSignalCallback()
        {
            testValue++;
        }

        private void OneArgSignalCallback(int argInt)
        {
            testValue += argInt;
        }

        private void OneArgSignalCallbackTwo(int argInt)
        {
            testValue++;
        }
	}
}
