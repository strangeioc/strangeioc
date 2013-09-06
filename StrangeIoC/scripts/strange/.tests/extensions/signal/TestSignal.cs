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
            testValue = 0;
        }

        private int testValue = 0;
        private int testInt = 1;
        private int testIntTwo = 2;
        private int testIntThree = 3;
        private int testIntFour = 4;

        [Test]
        public void TestNoArgSignal()
        {
            Signal signal = new Signal();

            signal.AddListener(NoArgSignalCallback);
            signal.Dispatch();
            Assert.AreEqual(1, testValue);
        }
        [Test]
        public void TestOneArgSignal()
        {
            Signal<int> signal = new Signal<int>();

            signal.AddListener(OneArgSignalCallback);
            signal.Dispatch(testInt);
            Assert.AreEqual(testInt, testValue);
        }

        //These are testing base functions, but also that ordering is correct using mathematical operators
        [Test]
        public void TestTwoArgSignal()
        {
            Signal<int, int> signal = new Signal<int, int>();

            signal.AddListener(TwoArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo);
            Assert.AreEqual(testInt + testIntTwo, testValue);

            signal.RemoveListener(TwoArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo);
            Assert.AreEqual(testInt + testIntTwo, testValue); //Removed listener should have no-op

            signal.AddOnce(TwoArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo);
            Assert.AreEqual((testInt + testIntTwo) * 2, testValue);

            signal.Dispatch(testInt, testIntTwo);
            Assert.AreEqual((testInt + testIntTwo) * 2, testValue); //addonce should result in no-op

        }
        [Test]
        public void TestThreeArgSignal()
        {
            Signal<int, int, int> signal = new Signal<int, int, int>();

            int intendedResult = (testInt + testIntTwo) * testIntThree;
            signal.AddListener(ThreeArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo, testIntThree);
            Assert.AreEqual(intendedResult, testValue);

            signal.RemoveListener(ThreeArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo, testIntThree);
            Assert.AreEqual(intendedResult, testValue); //no-op due to remove

            intendedResult += testInt;
            intendedResult += testIntTwo;
            intendedResult *= testIntThree;

            signal.AddOnce(ThreeArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo, testIntThree);
            Assert.AreEqual(intendedResult, testValue);

            signal.Dispatch(testInt, testIntTwo, testIntThree);
            Assert.AreEqual(intendedResult, testValue); //Add once should result in no-op
        }
        [Test]
        public void TestFourArgSignal()
        {
            Signal<int, int, int, int> signal = new Signal<int, int, int, int>();

            int intendedResult = ((testInt + testIntTwo) * testIntThree) - testIntFour;
            signal.AddListener(FourArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);
            Assert.AreEqual(intendedResult, testValue);

            signal.RemoveListener(FourArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);
            Assert.AreEqual(intendedResult, testValue); //no-op due to remove

            intendedResult += testInt;
            intendedResult += testIntTwo;
            intendedResult *= testIntThree;
            intendedResult -= testIntFour;

            signal.AddOnce(FourArgSignalCallback);
            signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);
            Assert.AreEqual(intendedResult, testValue);

            signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);
            Assert.AreEqual(intendedResult, testValue); //Add once should result in no-op
        }
        [Test]
        public void TestOnce()
        {
            Signal<int> signal = new Signal<int>();

            signal.AddOnce(OneArgSignalCallback);
            signal.Dispatch(testInt);
            Assert.AreEqual(testInt, testValue);

            signal.Dispatch(testInt);
            Assert.AreEqual(testInt, testValue); //should not fire a second time
        }

        [Test]
        public void TestMultipleCallbacks()
        {
            Signal<int> signal = new Signal<int>();

            signal.AddListener(OneArgSignalCallback);
            signal.AddListener(OneArgSignalCallbackTwo);

            signal.Dispatch(testInt);

            Assert.AreEqual(testInt + 1, testValue);
        }

        [Test]
        public void TestRemoveListener()
        {
            Signal<int> signal = new Signal<int>();

            signal.AddListener(OneArgSignalCallback);

            signal.Dispatch(testInt);
            Assert.AreEqual(testInt, testValue);

            signal.RemoveListener(OneArgSignalCallback);
            signal.Dispatch(testInt);
            Assert.AreEqual(testInt, testValue); //should not have changed
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
        private void TwoArgSignalCallback(int one, int two)
        {
            testValue += one;
            testValue += two;
        }

        private void ThreeArgSignalCallback(int one, int two, int three)
        {
            testValue += one;
            testValue += two;
            testValue *= three;
        }

        private void FourArgSignalCallback(int one, int two, int three, int four)
        {
            testValue += one;
            testValue += two;
            testValue *= three;
            testValue -= four;
        }
	}
}
