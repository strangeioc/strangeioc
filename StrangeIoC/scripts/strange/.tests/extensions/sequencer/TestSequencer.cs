using System;
using NUnit.Framework;
using strange.extensions.sequencer.api;
using strange.extensions.sequencer.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;

using strange.framework.impl;
using strange.extensions.command.api;
using strange.framework.api;


namespace strange.unittests
{
	[TestFixture()]
	public class TestSequencer
	{
		IInjectionBinder injectionBinder;
		ISequencer sequencer;

		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IInjectionBinder> ().Bind<IInstanceProvider> ().ToValue (injectionBinder);
			injectionBinder.Bind<ISequencer> ().Bind<ICommandBinder>().To<Sequencer> ().ToSingleton ();
			sequencer = injectionBinder.GetInstance<ISequencer> () as ISequencer;
		}

		[Test]
		public void TestExecuteWithInjection ()
		{
			//SequencerCommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			sequencer.Bind(SomeEnum.ONE).To<SequenceCommandWithInjection>();
			sequencer.ReactTo (SomeEnum.ONE);

			//The command should set the value to 100
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestSequence ()
		{
			//CommandWithInjection requires an ISimpleInterface
			injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer> ().ToSingleton();

			//Bind the trigger to the command
			sequencer.Bind(SomeEnum.ONE).To<SequenceCommandWithInjection>().To<SequenceCommandWithExecute>().To<SequenceCommandThatThrows>();

			TestDelegate testDelegate = delegate 
			{
				sequencer.ReactTo (SomeEnum.ONE);
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
			sequencer.Bind(SomeEnum.ONE).To<SequenceCommandWithInjection>().To<SequenceInterruptingCommand>().To<SequenceCommandThatThrows>();

			TestDelegate testDelegate = delegate 
			{
				sequencer.ReactTo (SomeEnum.ONE);
			};

			//That the exception is not thrown demonstrates that the last command was interrupted
			Assert.DoesNotThrow (testDelegate);

			//That the value is 100 demonstrates that the first command ran
			ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
			Assert.AreEqual (100, instance.intValue);
		}

		[Test]
		public void TestFeedCommandToSequence ()
		{
			TestDelegate testDelegate = delegate 
			{
				sequencer.Bind (SomeEnum.ONE).To<CommandWithExecute> ();
			};
			//That the exception is thrown demonstrates that the last command ran
			SequencerException ex = Assert.Throws<SequencerException> (testDelegate);
			Assert.AreEqual (ex.type, SequencerExceptionType.COMMAND_USED_IN_SEQUENCE);
		}
	}
}

