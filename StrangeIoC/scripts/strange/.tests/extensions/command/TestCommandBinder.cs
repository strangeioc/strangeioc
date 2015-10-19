using System;
using System.Timers;
using NUnit.Framework;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestCommandBinder
	{
		IInjectionBinder injectionBinder;
		ICommandBinder commandBinder;

		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IInjectionBinder> ().Bind<IInstanceProvider> ().ToValue (injectionBinder);
			injectionBinder.Bind<ICommandBinder> ().To<CommandBinder> ().ToSingleton ();
			commandBinder = injectionBinder.GetInstance<ICommandBinder> ();
		}

		[Test]
		public void TestExecuteInjectionCommand ()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjection>();
			commandBinder.ReactTo (SomeEnum.ONE);

			//The command should set the value to 100
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>();
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestMultipleCommands ()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjection>().To<CommandWithExecute>().To<CommandThatThrows>();

			TestDelegate testDelegate = delegate 
			{
				commandBinder.ReactTo (SomeEnum.ONE);
			};

			//That the exception is thrown demonstrates that the last command ran
			NotImplementedException ex = Assert.Throws<NotImplementedException> (testDelegate);
			Assert.NotNull(ex);

			//That the value is 100 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestMultipleOfSame ()
		{
			injectionBinder.Bind<TestModel>().ToSingleton();
			commandBinder.Bind(SomeEnum.ONE).To<NoArgCommand>().To<NoArgCommand>();
			TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;
			Assert.AreEqual(0, testModel.Value);
			commandBinder.ReactTo (SomeEnum.ONE);
			Assert.AreEqual(2, testModel.Value); //first command gives 1, second gives 2
		}

		[Test]
		public void TestNotOnce()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjection>();

			ICommandBinding binding = commandBinder.GetBinding (SomeEnum.ONE) as ICommandBinding;
			Assert.IsNotNull (binding);

			commandBinder.ReactTo (SomeEnum.ONE);

			ICommandBinding binding2 = commandBinder.GetBinding (SomeEnum.ONE) as ICommandBinding;
			Assert.IsNotNull (binding2);
		}

		[Test]
		public void TestOnce ()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjection>().Once();

			ICommandBinding binding = commandBinder.GetBinding (SomeEnum.ONE) as ICommandBinding;
			Assert.IsNotNull (binding);

			commandBinder.ReactTo (SomeEnum.ONE);

			ICommandBinding binding2 = commandBinder.GetBinding (SomeEnum.ONE) as ICommandBinding;
			Assert.IsNull (binding2);
		}

		[Test]
		public void TestSequence ()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjection>().To<CommandWithExecute>().To<CommandThatThrows>().InSequence();

			TestDelegate testDelegate = delegate 
			{
				commandBinder.ReactTo (SomeEnum.ONE);
			};

			//That the exception is thrown demonstrates that the last command ran
			NotImplementedException ex = Assert.Throws<NotImplementedException> (testDelegate);
			Assert.NotNull(ex);

			//That the value is 100 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestInterruptedSequence ()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			commandBinder.Bind(SomeEnum.ONE).To<CommandWithInjection>().To<FailCommand>().To<CommandThatThrows>().InSequence();

			TestDelegate testDelegate = delegate 
			{
				commandBinder.ReactTo (SomeEnum.ONE);
			};

			//That the exception is not thrown demonstrates that the last command was interrupted
			Assert.DoesNotThrow (testDelegate);

			//That the value is 100 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestSimpleRuntimeCommandBinding()
		{
			string jsonInjectorString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\", \"Options\":\"ToSingleton\"}]";
			injectionBinder.ConsumeBindings (jsonInjectorString);

			string jsonCommandString = "[{\"Bind\":\"strange.unittests.SomeEnum.ONE\",\"To\":\"strange.unittests.CommandWithInjection\"}]";
			commandBinder.ConsumeBindings(jsonCommandString);
			commandBinder.ReactTo (SomeEnum.ONE);

			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestRuntimeSequenceCommandBinding()
		{
			string jsonInjectorString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\", \"Options\":\"ToSingleton\"}]";
			injectionBinder.ConsumeBindings (jsonInjectorString);

			string jsonCommandString = "[{\"Bind\":\"TestEvent\",\"To\":[\"strange.unittests.CommandWithInjection\",\"strange.unittests.CommandWithExecute\",\"strange.unittests.CommandThatThrows\"],\"Options\":\"InSequence\"}]";
			commandBinder.ConsumeBindings(jsonCommandString);

			ICommandBinding binding = commandBinder.GetBinding ("TestEvent") as ICommandBinding;
			Assert.IsTrue (binding.isSequence);

			TestDelegate testDelegate = delegate 
			{
				commandBinder.ReactTo ("TestEvent");
			};

			//That the exception is thrown demonstrates that the last command ran
			NotImplementedException ex = Assert.Throws<NotImplementedException> (testDelegate);
			Assert.NotNull(ex);

			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestRuntimeCommandBindingOnce()
		{
			string jsonInjectorString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\", \"Options\":\"ToSingleton\"}]";
			injectionBinder.ConsumeBindings (jsonInjectorString);

			string jsonCommandString = "[{\"Bind\":\"TestEvent\",\"To\":[\"strange.unittests.CommandWithInjection\"],\"Options\":\"Once\"}]";
			commandBinder.ConsumeBindings(jsonCommandString);

			ICommandBinding binding = commandBinder.GetBinding ("TestEvent") as ICommandBinding;
			Assert.IsTrue (binding.isOneOff);
			commandBinder.ReactTo ("TestEvent");

			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);

			ICommandBinding binding2 = commandBinder.GetBinding ("TestEvent") as ICommandBinding;
			Assert.IsNull (binding2);
		}

		[Test]
		public void TestRuntimeUnqualifiedCommandException()
		{
			string jsonInjectorString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\", \"Options\":\"ToSingleton\"}]";
			injectionBinder.ConsumeBindings (jsonInjectorString);

			string jsonCommandString = "[{\"Bind\":\"TestEvent\",\"To\":\"CommandWithInjection\"}]";
			TestDelegate testDelegate = delegate 
			{
				commandBinder.ConsumeBindings(jsonCommandString);
			};

			BinderException ex = Assert.Throws<BinderException> (testDelegate);
			Assert.AreEqual (ex.type, BinderExceptionType.RUNTIME_NULL_VALUE);
		}



		//TODO: figure out how to do async tests
		/*
		[Test]
		public async void TestAsyncCommand()
		{
			injectionBinder.Bind<Timer>().To<Timer> ();
			commandBinder.Bind (SomeEnum.ONE).To<AsynchCommand> ();
			Task<bool> answer = commandBinder.ReactTo (SomeEnum.ONE);

			//Assert.Throws<Exception> ( await );
		}
		*/
	}


	class NoArgCommand: Command
	{
		[Inject]
		public TestModel TestModel { get; set; }

		public override void Execute()
		{
			TestModel.Value++;
		}
	}
}

