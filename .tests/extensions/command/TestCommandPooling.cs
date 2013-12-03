
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
	public class TestCommandPooling
	{
		private ICommandBinder commandBinder;
		private IPooledCommandBinder pooledCommandBinder;
		private IInjectionBinder injectionBinder;


		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder ();
			injectionBinder.Bind<IInjectionBinder>().Bind<IInstanceProvider>().ToValue(injectionBinder);
			injectionBinder.Bind<ICommandBinder> ().To<CommandBinder> ().ToSingleton();

			commandBinder = injectionBinder.GetInstance<ICommandBinder> ();
			pooledCommandBinder = commandBinder as IPooledCommandBinder;
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
			commandBinder.Bind (SomeEnum.ONE).To<CommandWithInjection> ().Pooled();
			TestDelegate testDelegate = delegate 
			{
				commandBinder.ReactTo (SomeEnum.ONE);
			};
			//If the injected value were not set, this command would throw a Null Pointer Exception
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestCommandGetsReused()
		{
			commandBinder.Bind (SomeEnum.ONE).To<MarkablePoolCommand> ().Pooled();
			IPool<MarkablePoolCommand> pool = pooledCommandBinder.GetPool<MarkablePoolCommand> ();

			for (int a = 0; a < 10; a++)
			{
				commandBinder.ReactTo (SomeEnum.ONE);
				Assert.AreEqual (a+1, MarkablePoolCommand.incrementValue);
				Assert.AreEqual (1, pool.instanceCount);
			}
		}

		[Test]
		public void TestCommandBinderHasManyPools()
		{
			commandBinder.Bind (SomeEnum.ONE).To<MarkablePoolCommand> ().Pooled();
			commandBinder.Bind (SomeEnum.TWO).To<CommandWithExecute> ().Pooled();
			commandBinder.Bind (SomeEnum.THREE).To<SequenceCommandWithInjection> ().Pooled();

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
			commandBinder.Bind (SomeEnum.ONE).To<CommandWithInjection> ().Pooled();

			commandBinder.ReactTo (SomeEnum.ONE);
			IPool<CommandWithInjection> pool = pooledCommandBinder.GetPool<CommandWithInjection> ();

			CommandWithInjection cmd = pool.GetInstance () as CommandWithInjection;

			Assert.AreEqual (1, pool.instanceCount);	//These just assert our expectation that there's one instance
			Assert.AreEqual (0, pool.available);		//and we're looking at it.

			Assert.IsNull (cmd.injected);
		}

		[Test]
		public void TestCommandWorksSecondTime()
		{
			injectionBinder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ();
			commandBinder.Bind (SomeEnum.ONE).To<CommandWithInjection> ().Pooled();

			commandBinder.ReactTo (SomeEnum.ONE);
			IPool<CommandWithInjection> pool = pooledCommandBinder.GetPool<CommandWithInjection> ();

			CommandWithInjection cmd = pool.GetInstance () as CommandWithInjection;

			Assert.AreEqual (1, pool.instanceCount);	//These just assert our expectation that there's one instance
			Assert.AreEqual (0, pool.available);		//and we're looking at it.

			Assert.IsNull (cmd.injected);

			TestDelegate testDelegate = delegate 
			{
				commandBinder.ReactTo (SomeEnum.ONE);
			};
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestFactoryInjectionGivesUniqueInstances()
		{
			injectionBinder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ();
			injectionBinder.Bind<Signal<SimpleInterfaceImplementer>> ().To<Signal<SimpleInterfaceImplementer>> ().ToSingleton ();
			commandBinder.Bind (SomeEnum.ONE).To<CommandWithInjectionAndSignal> ().Pooled();

			Signal<SimpleInterfaceImplementer> signal = injectionBinder.GetInstance<Signal<SimpleInterfaceImplementer>>();
			signal.AddListener (cb);

			commandBinder.ReactTo (SomeEnum.ONE);
			commandBinder.ReactTo (SomeEnum.ONE);

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

