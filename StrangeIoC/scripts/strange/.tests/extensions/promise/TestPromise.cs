using System;
using NUnit.Framework;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

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
		private const int FailTwoValue = 1000;
		private const int ProgressTwoValue = 500;
		private const String exceptionStr = "Your code is bad and you should feel bad!";

		private float currentProgress = 0.0f;
		private Exception currentException = null;

		private IPromise promise;
		private IPromise<int> promiseOneArg;
		private IPromise<int, int> promiseTwoArg;
		private IPromise<int, int, int> promiseThreeArg;
		private IPromise<int, int, int, int> promiseFourArg;

		[SetUp]
		public void setup()
		{
			promise = new Promise();
			promiseOneArg = new Promise<int>();
			promiseTwoArg = new Promise<int, int>();
			promiseThreeArg = new Promise<int, int, int>();
			promiseFourArg = new Promise<int, int, int, int>();
			value = 0;
			currentException = null;
			currentProgress = 0.0f;
		}
		[Test]
		public void TestNoArgPromise()
		{
			promise.Then(NoArgCallback);
			promise.Dispatch();
			Assert.AreEqual(1, value);
		}

		[Test]
		public void TestOneArgPromise()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Dispatch(add);
			Assert.AreEqual(add, value);
		}

		[Test]
		public void TestTwoArgPromise()
		{
			promiseTwoArg.Then(TwoArgCallback);
			promiseTwoArg.Dispatch(add, addTwo);
			Assert.AreEqual(add+addTwo, value);
		}

		[Test]
		public void TestThreeArgPromise()
		{
			promiseThreeArg.Then(ThreeArgCallback);
			promiseThreeArg.Dispatch(add, addTwo, addThree);
			Assert.AreEqual(add + addTwo + addThree, value);
		}

		[Test]
		public void TestFourArgPromise()
		{
			promiseFourArg.Then(FourArgCallback);
			promiseFourArg.Dispatch(add, addTwo, addThree, addFour);
			Assert.AreEqual(add + addTwo + addThree + addFour, value);
		}

		[Test]
		public void TestDispatchFirst()
		{
			Assert.AreEqual(0, value);
			promiseOneArg.Dispatch(add);
			Assert.AreEqual(0, value);

			promiseOneArg.Then(OneArgCallback);
			Assert.AreEqual(add, value);
		}

		[Test]
		public void TestDispatchFirstTwo()
		{
			promiseTwoArg.Dispatch(add, addTwo);
			Assert.AreEqual(0, value);

			promiseTwoArg.Then(TwoArgCallback);
			Assert.AreEqual(add+addTwo, value);
		}

		[Test]
		public void TestDispatchFirstThree()
		{
			promiseThreeArg.Dispatch(add, addTwo, addThree);
			Assert.AreEqual(0, value);

			promiseThreeArg.Then(ThreeArgCallback);
			Assert.AreEqual(add + addTwo + addThree, value);
		}

		[Test]
		public void TestDispatchFirstFour()
		{
			promiseFourArg.Dispatch(add, addTwo, addThree, addFour);
			Assert.AreEqual(0, value);

			promiseFourArg.Then(FourArgCallback);
			Assert.AreEqual(add + addTwo + addThree + addFour, value);
		}

		[Test]
		public void TestRemoveListener()
		{
			promise.Then(NoArgCallback);
			promise.RemoveListener(NoArgCallback);

			promise.Dispatch();
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveListenerOne()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.RemoveListener(OneArgCallback);

			promiseOneArg.Dispatch(add);
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveListenerTwo()
		{
			promiseTwoArg.Then(TwoArgCallback);
			promiseTwoArg.RemoveListener(TwoArgCallback);

			promiseTwoArg.Dispatch(add, addTwo);
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveListenerThree()
		{
			promiseThreeArg.Then(ThreeArgCallback);
			promiseThreeArg.RemoveListener(ThreeArgCallback);

			promiseThreeArg.Dispatch(add, addTwo, addThree);
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveListenerFour()
		{
			promiseFourArg.Then(FourArgCallback);
			promiseFourArg.RemoveListener(FourArgCallback);

			promiseFourArg.Dispatch(add, addTwo, addThree, addFour);
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveOnlyRemovesThatListener()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Then(OneArgCallbackTwo);
			promiseOneArg.RemoveListener(OneArgCallback);

			promiseOneArg.Dispatch(add);
			Assert.AreEqual(add*2, value);
		}

		[Test]
		public void TestRemoveAllListeners()
		{
			promise.Then(NoArgCallback);
			promise.Then(NoArgCallbackTwo);
			promise.RemoveAllListeners();

			promise.Dispatch();
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestRemoveAllListenersOneArg()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Then(OneArgCallbackTwo);
			promiseOneArg.RemoveAllListeners();

			promiseOneArg.Dispatch(add);
			Assert.AreEqual(0, value);
		}
		[Test]
		public void TestRemoveAllListenersTwoArg()
		{
			promiseTwoArg.Then(TwoArgCallback);
			promiseTwoArg.RemoveAllListeners();

			promiseTwoArg.Dispatch(add, addTwo);
			Assert.AreEqual(0, value);
		}
		[Test]
		public void TestRemoveAllListenersThreeArg()
		{
			promiseThreeArg.Then(ThreeArgCallback);
			promiseThreeArg.RemoveAllListeners();

			promiseThreeArg.Dispatch(add, addTwo, addThree);
			Assert.AreEqual(0, value);
		}
		[Test]
		public void TestRemoveAllListenersFourArg()
		{
			promiseFourArg.Then(FourArgCallback);
			promiseFourArg.RemoveAllListeners();

			promiseFourArg.Dispatch(add, addTwo, addThree, addFour);
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestMultipleListeners()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Then(OneArgCallbackTwo);

			promiseOneArg.Dispatch(add);
			Assert.AreEqual(add+(add * 2), value);
		}

		[Test]
		public void TestThenOrder()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Then(OneArgCallbackTwo);
			promiseOneArg.Then(OneArgCallbackThree);

			promiseOneArg.Dispatch(add);
			Assert.AreEqual((add+(add * 2))*add, value);

			value = 0;

			promiseOneArg = new Promise<int>();
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Then(OneArgCallbackThree);
			promiseOneArg.Then(OneArgCallbackTwo);

			promiseOneArg.Dispatch(add);
			Assert.AreEqual((add*add)+(add*2), value);
		}

		[Test]
		public void TestRemoveFromMiddle()
		{
			promiseOneArg.Then(OneArgCallback);
			promiseOneArg.Then(OneArgCallbackTwo);
			promiseOneArg.Then(OneArgCallbackThree);

			promiseOneArg.RemoveListener(OneArgCallbackTwo);
			promiseOneArg.Dispatch(add);
			Assert.AreEqual(add * add, value);
		}


		[Test]
		public void TestProgress()
		{
			float progress = 0.5f;
			promise.Progress(ProgressCallback);
			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);
		}

		[Test]
		public void TestError()
		{
			promise.Fail(FailCallback);
			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(currentException.Message, exceptionStr);
		}

		[Test]
		public void TestErrorFirst()
		{
			promiseOneArg.ReportFail(new Exception(exceptionStr));
			promiseOneArg.Fail(FailCallback);
			Assert.AreEqual(currentException.Message, exceptionStr);
		}

		[Test]
		public void TestProgressAndError()
		{
			float progress = 0.5f;
			promise
				.Progress(ProgressCallback)
				.Fail(FailCallback);


			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(currentException.Message, exceptionStr);
		}

		[Test]
		public void TestProgressAndErrorOrderDoesntMatter()
		{
			float progress = 0.5f;
			promise
				.Fail(FailCallback)
				.Progress(ProgressCallback);


			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(currentException.Message, exceptionStr);
		}

		[Test]
		public void TestThenProgressAndError()
		{
			float progress = 0.25f;

			promise
				.Then(NoArgCallback)
				.Fail(FailCallback)
				.Progress(ProgressCallback);


			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);

			promise.Dispatch();
			Assert.AreEqual(1, value);
		}

		[Test]
		public void TestMultipleProgress()
		{

			promise
				.Then(NoArgCallback)
				.Fail(FailCallback)
				.Progress(ProgressCallback);


			float progress = 0.25f;
			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);

			progress = 0.5f;
			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);

			progress = 0.75f;
			promise.ReportProgress(progress);
			Assert.AreEqual(progress, currentProgress);

			promise.Dispatch();
			Assert.AreEqual(1, value);
		}


		[Test]
		public void TestFailDoesNotAllowThen()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback)
				.Progress(ProgressCallback);


			promise.ReportFail(new Exception(exceptionStr));
			promise.Then(NoArgCallbackTwo);

			Assert.AreEqual(0, promise.ListenerCount());
		}

		[Test]
		public void TestCannotAddListenerTwice()
		{
			promise
				.Then(NoArgCallback)
				.Then(NoArgCallback);

			Assert.AreEqual(1, promise.ListenerCount());
		}

		[Test]
		public void TestCannotAddListenerTwiceOne()
		{
			promiseOneArg
				.Then(OneArgCallback)
				.Then(OneArgCallback);

			Assert.AreEqual(1, promiseOneArg.ListenerCount());
		}
		[Test]
		public void TestCannotAddListenerTwiceTwo()
		{
			promiseTwoArg
				.Then(TwoArgCallback)
				.Then(TwoArgCallback);

			Assert.AreEqual(1, promiseTwoArg.ListenerCount());
		}

		[Test]
		public void TestCannotAddListenerTwiceThree()
		{
			promiseThreeArg
				.Then(ThreeArgCallback)
				.Then(ThreeArgCallback);

			Assert.AreEqual(1, promiseThreeArg.ListenerCount());
		}

		[Test]
		public void TestCannotAddListenerTwiceFour()
		{
			promiseFourArg
				.Then(FourArgCallback)
				.Then(FourArgCallback);

			Assert.AreEqual(1, promiseFourArg.ListenerCount());
		}

		[Test]
		public void TestCannotAddFailTwice()
		{
			promise
				.Fail(FailCallbackTwo)
				.Fail(FailCallbackTwo);
			
			promise.ReportFail(new Exception(exceptionStr));

				Assert.AreEqual(FailTwoValue, value);
		}

		[Test]
		public void TestCannotAddProgressTwice()
		{
			promise
				.Progress(ProgressCallbackTwo)
				.Progress(ProgressCallbackTwo);

			promise.ReportProgress(1.0f);
			Assert.AreEqual(ProgressTwoValue, value);
		}

		[Test]
		public void TestFinallyAfterDispatch()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback)
				.Progress(ProgressCallback)
				.Finally(FinallyCallback);


			promise.Dispatch();

			//values set by finally
			Assert.AreEqual(42, value);
			Assert.AreEqual(-42f, currentProgress);
		}

		[Test]
		public void TestFinallyAfterFail()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback)
				.Progress(ProgressCallback)
				.Finally(FinallyCallback);


			promise.ReportFail(new Exception(exceptionStr));

			//values set by finally
			Assert.AreEqual(42, value);
			Assert.AreEqual(-42f, currentProgress);
			Assert.AreEqual(currentException.Message, exceptionStr);
		}

		[Test]
		public void TestFinallyImmediate()
		{
			promise.Dispatch();

			promise
				.Then(NoArgCallback)
				.Fail(FailCallback)
				.Progress(ProgressCallback)
				.Finally(FinallyCallback);


			//values set by finally
			Assert.AreEqual(42, value);
			Assert.AreEqual(-42f, currentProgress);
		}


		[Test]
		public void TestRemoveProgress()
		{
			float progress = 0.5f;
			promise.Progress(ProgressCallback);

			promise.RemoveProgressListeners();

			promise.ReportProgress(progress);
			Assert.AreEqual(0.0f, currentProgress);
		}

		[Test]
		public void TestRemoveFail()
		{
			promise.Fail(FailCallback);
			promise.RemoveFailListeners();

			promise.ReportFail(new Exception(exceptionStr));
			Assert.IsNull(currentException);
		}

		[Test]
		public void TestSecondDispatchIsNoOp()
		{
			promise.Then(NoArgCallback);

			promise.Dispatch();
			Assert.AreEqual(1, value);

			promise.Dispatch();
			Assert.AreEqual(1, value); //second dispatch is no-op

			promise.Then(NoArgCallbackTwo);
			promise.Dispatch();
			Assert.AreEqual(3, value); //Should still allow for a Then to process, though!
		}

		[Test]
		public void TestSecondFailIsNoOp()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(exceptionStr, currentException.Message);
			
			currentException = null;
			promise.ReportFail(new Exception(exceptionStr));
			Assert.IsNull(currentException);
		}

		[Test]
		public void TestDispatchAfterFailIsNoOp()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(0, value);
			Assert.AreEqual(exceptionStr, currentException.Message);

			promise.Dispatch();
			Assert.AreEqual(0, value);
		}

		[Test]
		public void TestFailAfterDispatchIsNoOp()
		{
			
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback);

			promise.Dispatch();
			Assert.AreEqual(1, value);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(1, value);
			Assert.IsNull(currentException);
		}


		[Test]
		public void TestListenersAreClearedAfterDispatch()
		{
			promise.Then(NoArgCallback);
			promise.Dispatch();
			Assert.AreEqual(1, value);

			promise.Then(NoArgCallbackTwo);
			promise.Dispatch();
			Assert.AreEqual(3, value);
		}

		[Test]
		public void TestListenersAreClearedAfterFail()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.AreEqual(0, value);
			Assert.AreEqual(exceptionStr, currentException.Message);

			promise.Then(NoArgCallbackTwo);
			promise.Dispatch();
			Assert.AreEqual(0, value);
		}


		[Test]
		public void TestProgressAreClearedAfterDispatch()
		{
			promise.Progress(ProgressCallback);
			promise.ReportProgress(0.5f);
			Assert.AreEqual(0.5f, currentProgress);

			promise.Dispatch();

			promise.ReportProgress(0.75f);
			Assert.AreEqual(0.5f, currentProgress);
		}

		[Test]
		public void TestFailAreClearedAfterDispatch()
		{
			promise
				.Then(NoArgCallback)
				.Fail(FailCallback);

			promise.Dispatch();
			Assert.AreEqual(1, value);

			promise.ReportFail(new Exception(exceptionStr));
			Assert.IsNull(currentException);
		}

		[Test]
		public void TestFinallyAreClearedAfterDispatch()
		{
			promise
				.Then(NoArgCallback)
				.Finally(FinallyCallback);

			promise.Dispatch();
			Assert.AreEqual(42, value);
			
			value = 0;
			promise.Dispatch();
			Assert.AreEqual(0, value);
		}

#region Callbacks
		
		private void NoArgCallback()
		{
			value++;
		}

		private void NoArgCallbackTwo()
		{
			value += 2;
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

		private void ProgressCallback(float progress)
		{
			currentProgress = progress;
		}

		private void FailCallback(Exception ex)
		{
			currentException = ex;
		}

		private void FinallyCallback()
		{
			value = 42;
			currentProgress = -42f;
		}

		private void FailCallbackTwo(Exception ex)
		{
			value += FailTwoValue;
		}

		private void ProgressCallbackTwo(float a)
		{
			value += ProgressTwoValue;
		}

#endregion

	}
}
