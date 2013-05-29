using System;
using NUnit.Framework;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;


namespace babel.unittests
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
			dispatcher.addListener (SomeEnum.ONE, noArgumentsMethod);
			Assert.IsTrue (dispatcher.hasListener(SomeEnum.ONE, noArgumentsMethod));
		}

		[Test]
		public void TestAddListenerOneArg ()
		{
			dispatcher.addListener (SomeEnum.ONE, oneArgumentMethod);
			Assert.IsTrue (dispatcher.hasListener(SomeEnum.ONE, oneArgumentMethod));
		}

		[Test]
		public void TestRemoveListenerNoArgs()
		{
			dispatcher.addListener (SomeEnum.ONE, noArgumentsMethod);
			dispatcher.removeListener (SomeEnum.ONE, noArgumentsMethod);
			Assert.IsFalse (dispatcher.hasListener(SomeEnum.ONE, noArgumentsMethod));
		}

		[Test]
		public void TestRemoveListenerOneArg()
		{
			dispatcher.addListener (SomeEnum.ONE, oneArgumentMethod);
			dispatcher.removeListener (SomeEnum.ONE, oneArgumentMethod);
			Assert.IsFalse (dispatcher.hasListener(SomeEnum.ONE, oneArgumentMethod));
		}

		[Test]
		public void TestUpdateListenerNoArgs()
		{
			dispatcher.updateListener (true, SomeEnum.ONE, noArgumentsMethod);
			Assert.IsTrue (dispatcher.hasListener(SomeEnum.ONE, noArgumentsMethod));
			dispatcher.updateListener (false, SomeEnum.ONE, noArgumentsMethod);
			Assert.IsFalse (dispatcher.hasListener(SomeEnum.ONE, noArgumentsMethod));
		}

		[Test]
		public void TestUpdateListenerOneArg()
		{
			dispatcher.updateListener (true, SomeEnum.ONE, oneArgumentMethod);
			Assert.IsTrue (dispatcher.hasListener(SomeEnum.ONE, oneArgumentMethod));
			dispatcher.updateListener (false, SomeEnum.ONE, oneArgumentMethod);
			Assert.IsFalse (dispatcher.hasListener(SomeEnum.ONE, oneArgumentMethod));
		}

		[Test]
		public void TestDispatchNoArgs()
		{
			confirmationValue = INIT_VALUE;
			dispatcher.updateListener (true, SomeEnum.ONE, noArgumentsMethod);
			dispatcher.Dispatch (SomeEnum.ONE);
			Assert.AreEqual (INIT_VALUE + INCREMENT, confirmationValue);
		}

		[Test]
		public void TestDispatchOneArg()
		{
			confirmationValue = INIT_VALUE;
			dispatcher.updateListener (true, SomeEnum.ONE, oneArgumentMethod);
			dispatcher.Dispatch (SomeEnum.ONE, PAYLOAD);
			Assert.AreEqual (INIT_VALUE + PAYLOAD, confirmationValue);
		}

		private void noArgumentsMethod()
		{
			confirmationValue += INCREMENT;
		}

		private void oneArgumentMethod(object value)
		{
			confirmationValue += (int)value;
		}
	}
}

