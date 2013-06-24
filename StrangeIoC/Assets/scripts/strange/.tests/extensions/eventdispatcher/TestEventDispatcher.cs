using System;
using NUnit.Framework;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;


namespace strange.unittests
{
	[TestFixture()]
	public class TestEventDispatcher
	{
		IEventDispatcher dispatcher;
		private const int INIT_VALUE = 42;
		private const int INCREMENT = 4;
		private const int PAYLOAD = 8;
		private int confirmationValue = 42;

		[SetUp]
		public void SetUp()
		{
			dispatcher = new EventDispatcher ();
		}

		[Test]
		public void TestAddListenerNoArgs ()
		{
			dispatcher.AddListener (SomeEnum.ONE, noArgumentsMethod);
			Assert.IsTrue (dispatcher.HasListener(SomeEnum.ONE, noArgumentsMethod));
		}

		[Test]
		public void TestAddListenerOneArg ()
		{
			dispatcher.AddListener (SomeEnum.ONE, oneArgumentMethod);
			Assert.IsTrue (dispatcher.HasListener(SomeEnum.ONE, oneArgumentMethod));
		}

		[Test]
		public void TestRemoveListenerNoArgs()
		{
			dispatcher.AddListener (SomeEnum.ONE, noArgumentsMethod);
			dispatcher.RemoveListener (SomeEnum.ONE, noArgumentsMethod);
			Assert.IsFalse (dispatcher.HasListener(SomeEnum.ONE, noArgumentsMethod));
		}

		[Test]
		public void TestRemoveListenerOneArg()
		{
			dispatcher.AddListener (SomeEnum.ONE, oneArgumentMethod);
			dispatcher.RemoveListener (SomeEnum.ONE, oneArgumentMethod);
			Assert.IsFalse (dispatcher.HasListener(SomeEnum.ONE, oneArgumentMethod));
		}

		[Test]
		public void TestUpdateListenerNoArgs()
		{
			dispatcher.UpdateListener (true, SomeEnum.ONE, noArgumentsMethod);
			Assert.IsTrue (dispatcher.HasListener(SomeEnum.ONE, noArgumentsMethod));
			dispatcher.UpdateListener (false, SomeEnum.ONE, noArgumentsMethod);
			Assert.IsFalse (dispatcher.HasListener(SomeEnum.ONE, noArgumentsMethod));
		}

		[Test]
		public void TestUpdateListenerOneArg()
		{
			dispatcher.UpdateListener (true, SomeEnum.ONE, oneArgumentMethod);
			Assert.IsTrue (dispatcher.HasListener(SomeEnum.ONE, oneArgumentMethod));
			dispatcher.UpdateListener (false, SomeEnum.ONE, oneArgumentMethod);
			Assert.IsFalse (dispatcher.HasListener(SomeEnum.ONE, oneArgumentMethod));
		}

		[Test]
		public void TestDispatchNoArgs()
		{
			confirmationValue = INIT_VALUE;
			dispatcher.UpdateListener (true, SomeEnum.ONE, noArgumentsMethod);
			dispatcher.Dispatch (SomeEnum.ONE);
			Assert.AreEqual (INIT_VALUE + INCREMENT, confirmationValue);
		}

		[Test]
		public void TestDispatchOneArg()
		{
			confirmationValue = INIT_VALUE;
			dispatcher.UpdateListener (true, SomeEnum.ONE, oneArgumentMethod);
			dispatcher.Dispatch (SomeEnum.ONE, PAYLOAD);
			Assert.AreEqual (INIT_VALUE + PAYLOAD, confirmationValue);
		}

		[Test]
		public void TestMultipleListeners()
		{
			confirmationValue = INIT_VALUE;
			dispatcher.AddListener (SomeEnum.ONE, noArgumentsMethod);
			dispatcher.AddListener (SomeEnum.ONE, oneArgumentMethod);
			dispatcher.Dispatch (SomeEnum.ONE, PAYLOAD);

			Assert.AreEqual(INIT_VALUE + PAYLOAD + INCREMENT, confirmationValue);
		}

		[Test]
		public void TestBadlyFormedCallback()
		{
			confirmationValue = INIT_VALUE;
			dispatcher.AddListener (SomeEnum.ONE, badArgumentMethod);

			TestDelegate testDelegate = delegate() {
				dispatcher.Dispatch (SomeEnum.ONE, PAYLOAD);
			};

			EventDispatcherException ex = Assert.Throws<EventDispatcherException> (testDelegate);
			Assert.That (ex.type == EventDispatcherExceptionType.TARGET_INVOCATION);
		}

		private void noArgumentsMethod()
		{
			confirmationValue += INCREMENT;
		}

		private void oneArgumentMethod(IEvent evt)
		{
			int data = (int)evt.data;

			confirmationValue += data;
		}

		private void badArgumentMethod(object payload)
		{
			int data = (int)payload;

			confirmationValue += data;
		}
	}
}

