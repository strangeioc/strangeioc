
using System;
using System.Collections.Generic;
using NUnit.Framework;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.extensions.pool.impl;
using strange.extensions.pool.api;
using strange.framework.api;
using strange.extensions.signal.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestPooledSignalCommands
	{
		private ICommandBinder commandBinder;
		private IPooledCommandBinder pooledCommandBinder;
		private IInjectionBinder injectionBinder;
		private Signal<int> singleSignal;
		private Signal<int, string> doubleSignal;
		private Signal<int, string, SimpleInterfaceImplementer> tripleSignal;


		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder ();
			injectionBinder.Bind<IInjectionBinder>().Bind<IInstanceProvider>().ToValue(injectionBinder);
			injectionBinder.Bind<ICommandBinder> ().To<SignalCommandBinder> ().ToSingleton();

			commandBinder = injectionBinder.GetInstance<ICommandBinder> ();
			pooledCommandBinder = commandBinder as IPooledCommandBinder;
			singleSignal = new Signal<int>();
			doubleSignal = new Signal<int, string>();
			tripleSignal = new Signal<int, string, SimpleInterfaceImplementer>();
		}

		[TearDown]
		public void TearDown()
		{
			MarkablePoolCommand.incrementValue = 0;
		}

		[Test]
		public void TestCommandIsInjected()
		{
			injectionBinder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ();
			commandBinder.Bind(singleSignal).To<CommandWithInjectionPlusSignalPayload> ().Pooled();
			TestDelegate testDelegate = delegate 
			{
				singleSignal.Dispatch(27);
			};
			//If the injected value were not set, this command would throw a Null Pointer Exception
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestCommandGetsReused()
		{
			commandBinder.Bind (singleSignal).To<MarkablePoolCommand> ().Pooled();
			IPool<MarkablePoolCommand> pool = pooledCommandBinder.GetPool<MarkablePoolCommand> ();

			for (int a = 0; a < 10; a++)
			{
				singleSignal.Dispatch (a);
				Assert.AreEqual (a+1, MarkablePoolCommand.incrementValue);
				Assert.AreEqual (1, pool.instanceCount);
			}
		}

		[Test]
		public void TestCommandBinderHasManyPools()
		{
			commandBinder.Bind (singleSignal).To<MarkablePoolCommand> ().Pooled();
			commandBinder.Bind (doubleSignal).To<CommandWithExecute> ().Pooled();
			commandBinder.Bind (tripleSignal).To<SequenceCommandWithInjection> ().Pooled();

			IPool firstPool = pooledCommandBinder.GetPool<MarkablePoolCommand> ();
			IPool secondPool = pooledCommandBinder.GetPool<CommandWithExecute> ();
			IPool thirdPool = pooledCommandBinder.GetPool<SequenceCommandWithInjection> ();

			Assert.IsNotNull (firstPool);
			Assert.IsNotNull (secondPool);
			Assert.IsNotNull (thirdPool);

			Assert.AreNotSame (firstPool, secondPool);
			Assert.AreNotSame (secondPool, thirdPool);
			Assert.AreNotSame (thirdPool, firstPool);
		}

		[Test]
		public void TestCleanupInjections()
		{
			injectionBinder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ();
			commandBinder.Bind (singleSignal).To<CommandWithInjectionPlusSignalPayload> ().Pooled();

			singleSignal.Dispatch (42);
			IPool<CommandWithInjectionPlusSignalPayload> pool = pooledCommandBinder.GetPool<CommandWithInjectionPlusSignalPayload> ();

			CommandWithInjectionPlusSignalPayload cmd = pool.GetInstance() as CommandWithInjectionPlusSignalPayload;

			Assert.AreEqual (1, pool.instanceCount);	//These just assert our expectation that there's one instance
			Assert.AreEqual (0, pool.available);		//and we're looking at it.

			Assert.IsNull (cmd.injected);
		}

		[Test]
		public void TestCommandWorksSecondTime()
		{
			injectionBinder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ();
			commandBinder.Bind (singleSignal).To<CommandWithInjectionPlusSignalPayload> ().Pooled();

			singleSignal.Dispatch (42);
			IPool<CommandWithInjectionPlusSignalPayload> pool = pooledCommandBinder.GetPool<CommandWithInjectionPlusSignalPayload> ();

			CommandWithInjectionPlusSignalPayload cmd = pool.GetInstance () as CommandWithInjectionPlusSignalPayload;

			Assert.AreEqual (1, pool.instanceCount);	//These just assert our expectation that there's one instance
			Assert.AreEqual (0, pool.available);		//and we're looking at it.

			Assert.IsNull (cmd.injected);

			TestDelegate testDelegate = delegate 
			{
				singleSignal.Dispatch (42);
			};
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestFactoryInjectionGivesUniqueInstances()
		{
			injectionBinder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ();

			//This signal we dispatch
			Signal<Signal<SimpleInterfaceImplementer>> senderSignal = new Signal<Signal<SimpleInterfaceImplementer>> ();
			//This one is payload
			Signal<SimpleInterfaceImplementer> payloadSignal = new Signal<SimpleInterfaceImplementer> ();

			commandBinder.Bind (senderSignal).To<CommandWithInjectionAndSignal> ().Pooled();

			payloadSignal.AddListener (cb);

			//Dispatch the senderSignal twice, each time carrying the payloadSignal
			senderSignal.Dispatch(payloadSignal);
			senderSignal.Dispatch(payloadSignal);

			Assert.AreEqual (2, instanceList.Count);
			Assert.AreNotSame (instanceList [0], instanceList [1]);

		}

		private List<SimpleInterfaceImplementer> instanceList = new List<SimpleInterfaceImplementer>();
		private void cb(SimpleInterfaceImplementer instance)
		{
			instanceList.Add (instance);
		}
	}
}

