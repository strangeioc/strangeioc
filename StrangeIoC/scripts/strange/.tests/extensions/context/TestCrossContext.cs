using NUnit.Framework;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using System;
using strange.extensions.context.impl;
using System.Collections;
using UnityEngine;

namespace strange.unittests
{

    /**
     * Some functionality for Cross Context will be impossible to test
     * The goal here is to test whatever we can, but the actual implementation will 
     * vary per users and rely heavily on Unity in most cases, which we can't test here.
     **/ 
	[TestFixture]
	public class TestCrossContext
	{
        object view;
        [SetUp]
        public void SetUp()
        {
            Context.firstContext = null;
            view = new object();
            Parent = new CrossContext(view, true);
            ChildOne = new CrossContext(view, true);
            ChildTwo = new CrossContext(view, true);
        }

        CrossContext Parent;
        CrossContext ChildOne;
        CrossContext ChildTwo;
        
		[Test]
		public void TestCrossInjectionFromValue()
		{
            MyTestSignal parentSignal = new MyTestSignal();
            Parent.injectionBinder.Bind<MyTestSignal>().ToValue(parentSignal).CrossContext(); //bind it once here and it should be accessible everywhere

            MyTestSignal parentSignalTwo = Parent.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;

            Assert.AreEqual(parentSignal, parentSignalTwo); //Assure that this value is correct

            MyTestSignal signal = ChildOne.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;
            Assert.IsNotNull(signal);
            MyTestSignal sameSignal = ChildTwo.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;
            Assert.IsNotNull(sameSignal);
            Assert.AreEqual(signal, sameSignal); //These two should be the same object

            signal.AddOnce(TestCallback);
            TestDelegate testDelegate = delegate
            {
                parentSignal.Dispatch();
            };

            Assert.Throws<TestPassedException>(testDelegate); //first, addonce should handle removal. Tested elsewhere
            sameSignal.AddOnce(TestCallback);
            Assert.Throws<TestPassedException>(testDelegate); //With the second one
		}

        [Test]
        public void TestCrossInjectionFromSingleton()
        {
            Parent.injectionBinder.Bind<MyTestSignal>().ToSingleton().CrossContext(); //bind it once here and it should be accessible everywhere
            MyTestSignal parentSignal = Parent.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;
            Assert.IsNotNull(parentSignal);

            TestDelegate testDelegate = delegate
            {
                parentSignal.Dispatch();
            };


            MyTestSignal childOneSignal = ChildOne.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;
            MyTestSignal childTwoSignal = ChildTwo.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;
            Assert.IsNotNull(childOneSignal);
            Assert.IsNotNull(childTwoSignal);

            Assert.AreEqual(parentSignal, childOneSignal); //These two should be the same object
            Assert.AreEqual(parentSignal, childTwoSignal);
            Assert.AreEqual(childOneSignal, childTwoSignal);

            childOneSignal.AddOnce(TestCallback);
            Assert.Throws<TestPassedException>(testDelegate);

            childTwoSignal.AddOnce(TestCallbackTwo);
            Assert.Throws<TestPassedException>(testDelegate);
        }

        [Test]
        public void TestSingleton()
        {
            Parent.injectionBinder.Bind<MyTestSignal>().ToSingleton();

            MyTestSignal one = Parent.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;
            MyTestSignal two = Parent.injectionBinder.GetInstance<MyTestSignal>() as MyTestSignal;

            Assert.AreEqual(one, two);
        }

        
        private void TestCallback()
        {
            throw new TestPassedException("Test One Passed");
        }

        private void TestCallbackTwo()
        {
            throw new TestPassedException("Test Two Passed");
        }

        class TestPassedException : Exception
        {
            public TestPassedException(string exception) : base(exception)
            {
                
            }
        }


	}

    public class MyTestSignal : Signal
    {
    }
}
