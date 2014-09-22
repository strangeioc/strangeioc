using System;
using System.Collections.Generic;
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
        private int intToIncrement;

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
        public void GetTypes_ThreeType_ExpectsTypesReturnedInList()
        {
            Signal<int, string, float> signal = new Signal<int, string, float>();

            var actual = signal.GetTypes();

            var expected = this.GetThreeExpectedTypes();
            Assert.AreEqual(expected, actual);
        }

        private IList<Type> GetThreeExpectedTypes()
        {
            var expected = new List<Type>();
            expected.Add(typeof(int));
            expected.Add(typeof(string));
            expected.Add(typeof(float));
            return expected;
        }

        [Test]
        public void GetTypes_FourType_ExpectsTypesReturnedInList()
        {
            Signal<int, string, float, Signal> signal = new Signal<int, string, float, Signal>();

            var actual = signal.GetTypes();

            var expected = this.GetThreeExpectedTypes();
            expected.Add(typeof(Signal));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddListener_SignalWithNoTypeGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal signal = new Signal();
            intToIncrement = 0;
            signal.AddListener(SimpleSignalCallback);
            signal.AddListener(SimpleSignalCallback);

            signal.Dispatch();

            Assert.AreEqual(1, intToIncrement);
        }

        [Test]
        public void AddOnce_SignalWithNoTypeGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal signal = new Signal();
            intToIncrement = 0;
            signal.AddOnce(SimpleSignalCallback);
            signal.AddOnce(SimpleSignalCallback);

            signal.Dispatch();

            Assert.AreEqual(1, intToIncrement);
        }

        private void SimpleSignalCallback()
        {
            intToIncrement++;
        }

        [Test]
        public void AddListener_SignalWithOneTypeGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int> signal = new Signal<int>();
            testInt = 42;
            signal.AddListener(OneArgSignalCallback);
            signal.AddListener(OneArgSignalCallback);

            signal.Dispatch(testInt);

            Assert.AreEqual(testInt, testValue);
        }

        [Test]
        public void AddOnce_SignalWithOneTypeGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int> signal = new Signal<int>();
            testInt = 42;
            signal.AddOnce(OneArgSignalCallback);
            signal.AddOnce(OneArgSignalCallback);

            signal.Dispatch(testInt);

            Assert.AreEqual(testInt, testValue);
        }

        [Test]
        public void AddListener_SignalWithTwoTypesGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int, int> signal = new Signal<int, int>();
            this.CreateTwoInts();
            signal.AddListener(TwoArgCallback);
            signal.AddListener(TwoArgCallback);

            signal.Dispatch(testInt, testIntTwo);

            int expected = this.GetTwoIntExpected();
            Assert.AreEqual(expected, this.testValue);
        }

        [Test]
        public void AddOnce_SignalWithTwoTypesGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int, int> signal = new Signal<int, int>();
            this.CreateTwoInts();
            signal.AddOnce(TwoArgCallback);
            signal.AddOnce(TwoArgCallback);

            signal.Dispatch(testInt, testIntTwo);

            int expected = this.GetTwoIntExpected();
            Assert.AreEqual(expected, this.testValue);
        }

        private int GetTwoIntExpected()
        {
            return testInt + testIntTwo;
        }

        private void TwoArgCallback(int arg1, int arg2)
        {
            this.testValue += arg1 + arg2;
        }

        [Test]
        public void AddListener_SignalWithThreeTypesGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int, int, int> signal = new Signal<int, int, int>();
            this.CreateThreeInts();
            signal.AddListener(ThreeArgCallback);
            signal.AddListener(ThreeArgCallback);

            signal.Dispatch(testInt, testIntTwo, testIntThree);

            int expected = this.GetThreeIntExpected();
            Assert.AreEqual(expected, this.testValue);

        }

        [Test]
        public void AddOnce_SignalWithThreeTypesGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int, int, int> signal = new Signal<int, int, int>();
            this.CreateThreeInts();
            signal.AddOnce(ThreeArgCallback);
            signal.AddOnce(ThreeArgCallback);

            signal.Dispatch(testInt, testIntTwo, testIntThree);

            int expected = this.GetThreeIntExpected();
            Assert.AreEqual(expected, this.testValue);

        }

        private void ThreeArgCallback(int arg1, int arg2, int arg3)
        {
            this.testValue += arg1 + arg2 + arg3;
        }

        private int GetThreeIntExpected()
        {
            return this.GetTwoIntExpected() + testIntThree;
        }

        [Test]
        public void AddListener_SignalWithFourTypesGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int, int, int, int> signal = new Signal<int, int, int, int>();
            this.CreateFourInts();
            signal.AddListener(FourArgCallback);
            signal.AddListener(FourArgCallback);

            signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);

            int expected = this.GetFourIntExpected();
            Assert.AreEqual(expected, this.testValue);
        }

        [Test]
        public void AddOnce_SignalWithFourTypesGivenSameCallbackMultipleTimes_ExpectsDelegateCalledOnlyOnce()
        {
            Signal<int, int, int, int> signal = new Signal<int, int, int, int>();
            this.CreateFourInts();
            signal.AddOnce(FourArgCallback);
            signal.AddOnce(FourArgCallback);

            signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);

            int expected = this.GetFourIntExpected();
            Assert.AreEqual(expected, this.testValue);

        }

        private int GetFourIntExpected()
        {
            return this.GetThreeIntExpected() + testIntFour;
        }

        private void CreateFourInts()
        {
            this.CreateThreeInts();
            this.testIntFour = 4;
        }

        private void CreateThreeInts()
        {
            this.CreateTwoInts();
            this.testIntThree = 3;
        }

        private void CreateTwoInts()
        {
            this.CreateOneInt();
            this.testIntTwo = 2;
        }

        private void CreateOneInt()
        {
            this.testInt = 1;
        }

        private void FourArgCallback(int arg1, int arg2, int arg3, int arg4)
        {
            this.testValue += arg1 + arg2 + arg3 + arg4;
        }

		
		private Signal addOnceZeroArgSignal;
		[Test]
        public void AddOnce_SignalWithNoTypeGivenCallbackThatAddsItselfAgain_ExpectsDelegateCalledTwice()
        {
			addOnceZeroArgSignal = new Signal();
            intToIncrement = 0;
			addOnceZeroArgSignal.AddOnce(ZeroArgCallbackTriggeringAddOnceAgain);

			addOnceZeroArgSignal.Dispatch();
			addOnceZeroArgSignal.Dispatch();

            Assert.AreEqual(2, intToIncrement);
        }
		private void ZeroArgCallbackTriggeringAddOnceAgain()
		{
			intToIncrement++;
			addOnceZeroArgSignal.AddOnce(ZeroArgCallbackTriggeringAddOnceAgain);
		}

		private Signal<int> addOnceOneArgSignal;
		[Test]
        public void AddOnce_SignalWithOneTypeGivenCallbackThatAddsItselfAgain_ExpectsDelegateCalledTwice()
        {
			addOnceOneArgSignal = new Signal<int>();
			addOnceOneArgSignal.AddOnce(OneArgCallbackTriggeringAddOnceAgain);

			CreateOneInt();

			addOnceOneArgSignal.Dispatch(testInt);
			addOnceOneArgSignal.Dispatch(testInt);

            Assert.AreEqual(2*testInt, testValue);
        }
		private void OneArgCallbackTriggeringAddOnceAgain(int testInt)
		{
			testValue += testInt;
			addOnceOneArgSignal.AddOnce(OneArgCallbackTriggeringAddOnceAgain);
		}

		private Signal<int,int> addOnceTwoArgSignal;
		[Test]
        public void AddOnce_SignalWithTwoTypesGivenCallbackThatAddsItselfAgain_ExpectsDelegateCalledTwice()
        {
			addOnceTwoArgSignal = new Signal<int, int>();
			addOnceTwoArgSignal.AddOnce(TwoArgCallbackTriggeringAddOnceAgain);

			CreateTwoInts();

			addOnceTwoArgSignal.Dispatch(testInt, testIntTwo);
			addOnceTwoArgSignal.Dispatch(testInt, testIntTwo);

            Assert.AreEqual(2*(testInt+testIntTwo), testValue);
        }
		private void TwoArgCallbackTriggeringAddOnceAgain(int testInt, int testIntTwo)
		{
			testValue += testInt + testIntTwo;
			addOnceTwoArgSignal.AddOnce(TwoArgCallbackTriggeringAddOnceAgain);
		}

		private Signal<int,int,int> addOnceThreeArgSignal;
		[Test]
        public void AddOnce_SignalWithThreeTypesGivenCallbackThatAddsItselfAgain_ExpectsDelegateCalledTwice()
        {
			addOnceThreeArgSignal = new Signal<int, int, int>();
			addOnceThreeArgSignal.AddOnce(ThreeArgCallbackTriggeringAddOnceAgain);

			CreateThreeInts();

			addOnceThreeArgSignal.Dispatch(testInt, testIntTwo, testIntThree);
			Console.WriteLine ("1." + testValue);
			addOnceThreeArgSignal.Dispatch(testInt, testIntTwo, testIntThree);

			Assert.AreEqual(2*(testInt+testIntTwo+testIntThree), testValue);
        }
		private void ThreeArgCallbackTriggeringAddOnceAgain(int testInt, int testIntTwo, int testIntThree)
		{
			testValue += (testInt + testIntTwo + testIntThree);
			Console.WriteLine ("?? " + testValue);
			addOnceThreeArgSignal.AddOnce(ThreeArgCallbackTriggeringAddOnceAgain);
		}

		private Signal<int,int,int,int> addOnceFourArgSignal;
		[Test]
        public void AddOnce_SignalWithFourTypesGivenCallbackThatAddsItselfAgain_ExpectsDelegateCalledTwice()
        {
			addOnceFourArgSignal = new Signal<int, int, int, int>();
			addOnceFourArgSignal.AddOnce(FourArgCallbackTriggeringAddOnceAgain);

			CreateFourInts();

			addOnceFourArgSignal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);
			addOnceFourArgSignal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);

			Assert.AreEqual(2*(testInt+testIntTwo+testIntThree+testIntFour), testValue);
        }
		private void FourArgCallbackTriggeringAddOnceAgain(int testInt, int testIntTwo, int testIntThree, int testIntFour)
		{
			testValue += testInt + testIntTwo + testIntThree + testIntFour;
			addOnceFourArgSignal.AddOnce(FourArgCallbackTriggeringAddOnceAgain);
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
            Assert.AreEqual(testInt, testValue);
        }

        [Test]
        public void RemoveListener_NoType_ExpectsListenerRemoved()
        {
            Signal signal = new Signal();

            signal.AddListener(NoArgSignalCallback);

            signal.Dispatch();
            Assert.AreEqual(1, testValue);

            signal.RemoveListener(NoArgSignalCallback);
            signal.Dispatch();
            Assert.AreEqual(1, testValue);
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
