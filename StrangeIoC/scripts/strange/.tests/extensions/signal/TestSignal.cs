using System;
using System.Collections.Generic;
using NUnit.Framework;
using strange.extensions.signal.api;
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
		private const int baseSignalValue = 1000;

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
		public void TestBaseOnce()
		{
			BaseSignal signal = new BaseSignal();
			signal.AddOnce(BaseSignalCallback);
			
			signal.Dispatch(new object[] {});
			Assert.AreEqual(testValue, baseSignalValue);

			signal.Dispatch(new object[] { });
			Assert.AreEqual(testValue, baseSignalValue);
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

#region RemoveAllListeners

		[Test]
		public void TestRemoveAllListeners()
		{
			Signal signal = new Signal();

			signal.AddListener(NoArgSignalCallback);
			signal.AddListener(NoArgSignalCallbackTwo);

			signal.RemoveAllListeners();
			signal.Dispatch();

			Assert.AreEqual(0, testValue);
		}
		[Test]
		public void TestRemoveAllListenersOne()
		{
			Signal<int> signal = new Signal<int>();

			signal.AddListener(OneArgSignalCallback);
			signal.AddListener(OneArgSignalCallbackTwo);

			signal.RemoveAllListeners();
			signal.Dispatch(testInt);

			Assert.AreEqual(0, testValue);
		}
		[Test]
		public void TestRemoveAllListenersTwo()
		{
			Signal<int, int> signal = new Signal<int, int>();

			signal.AddListener(TwoArgSignalCallback);
			signal.AddListener(TwoArgSignalCallbackTwo);

			signal.RemoveAllListeners();
			signal.Dispatch(testInt, testIntTwo);

			Assert.AreEqual(0, testValue);
		}
		[Test]
		public void TestRemoveAllListenersThree()
		{
			Signal<int, int, int> signal = new Signal<int, int, int>();

			signal.AddListener(ThreeArgSignalCallback);
			signal.AddListener(ThreeArgSignalCallbackTwo);

			signal.RemoveAllListeners();
			signal.Dispatch(testInt, testIntTwo, testIntThree);

			Assert.AreEqual(0, testValue);
		}
		[Test]
		public void TestRemoveAllListenersFour()
		{
			Signal<int, int, int, int> signal = new Signal<int, int, int, int>();

			signal.AddListener(FourArgSignalCallback);
			signal.AddListener(FourArgSignalCallbackTwo);

			signal.RemoveAllListeners();
			signal.Dispatch(testInt, testIntTwo, testIntThree, testIntFour);

			Assert.AreEqual(0, testValue);
		}

		[Test]
		public void TestRemoveAllRemovesOnce()
		{
			Signal signal = new Signal();
			signal.AddOnce(NoArgSignalCallback);
			signal.AddOnce(NoArgSignalCallbackTwo);

			signal.RemoveAllListeners();
			signal.Dispatch();

			Assert.AreEqual(0, testValue);
		}

		[Test]
		public void TestRemoveListenerDoesntBlowUp()
		{
			Signal signal = new Signal();
			signal.RemoveListener(NoArgSignalCallback);
		}
		[Test]
		public void TestRemoveListenerDoesntBlowUpOne()
		{
			Signal<int> signal = new Signal<int>();
			signal.RemoveListener(OneArgSignalCallback);
		}
		[Test]
		public void TestRemoveListenerDoesntBlowUpTwo()
		{
			Signal<int,int> signal = new Signal<int,int>();
			signal.RemoveListener(TwoArgSignalCallback);
		}
		[Test]
		public void TestRemoveListenerDoesntBlowUpThree()
		{
			Signal<int,int,int> signal = new Signal<int,int,int>();
			signal.RemoveListener(ThreeArgSignalCallback);
		}
		[Test]
		public void TestRemoveListenerDoesntBlowUpFour()
		{
			Signal<int,int,int,int> signal = new Signal<int,int,int,int>();
			signal.RemoveListener(FourArgSignalCallback);
		}

#endregion

#region GetTypes

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
#endregion

#region NoCallbackDuplication
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

#endregion

#region callback

		private void NoArgSignalCallback()
		{
			testValue++;
		}

		private void NoArgSignalCallbackTwo()
		{
			testValue += 10;
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

		private void TwoArgSignalCallbackTwo(int one, int two)
		{
			testValue *= one;
			testValue *= two;
		}

		private void ThreeArgSignalCallback(int one, int two, int three)
		{
			testValue += one;
			testValue += two;
			testValue *= three;
		}
		private void ThreeArgSignalCallbackTwo(int one, int two, int three)
		{
			testValue *= one;
			testValue *= two;
			testValue *= three;
		}

		private void FourArgSignalCallback(int one, int two, int three, int four)
		{
			testValue += one;
			testValue += two;
			testValue *= three;
			testValue -= four;
		}
		private void FourArgSignalCallbackTwo(int one, int two, int three, int four)
		{
			testValue *= one;
			testValue *= two;
			testValue *= three;
			testValue *= four;
		}

		private void SimpleSignalCallback()
		{
			intToIncrement++;
		}

		private int GetTwoIntExpected()
		{
			return testInt + testIntTwo;
		}

		private void TwoArgCallback(int arg1, int arg2)
		{
			this.testValue += arg1 + arg2;
		}

		private void ThreeArgCallback(int arg1, int arg2, int arg3)
		{
			this.testValue += arg1 + arg2 + arg3;
		}

		private int GetThreeIntExpected()
		{
			return this.GetTwoIntExpected() + testIntThree;
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

		private void BaseSignalCallback(IBaseSignal signal, object[] args)
		{
			testValue += baseSignalValue;
		}

		#endregion


	}
}
