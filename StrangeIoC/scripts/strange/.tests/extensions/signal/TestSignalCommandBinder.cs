using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.command.impl;
using strange.extensions.command.api;
using strange.extensions.signal.impl;
using strange.extensions.signal.api;

namespace strange.unittests
{

	[TestFixture]
	public class TestSignalCommandBinder
	{

        IInjectionBinder injectionBinder;
        ICommandBinder commandBinder;

        [SetUp]
        public void SetUp()
        {
            injectionBinder = new InjectionBinder();
            injectionBinder.Bind<IInjectionBinder>().ToValue(injectionBinder);
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
            commandBinder = injectionBinder.GetInstance<ICommandBinder>() as ICommandBinder;
            injectionBinder.Bind<TestModel>().ToSingleton();
        }

        [Test]
        public void TestNoArgs()
        {
            commandBinder.Bind<NoArgSignal>().To<NoArgSignalCommand>();

            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.AreEqual(testModel.StoredValue, 0);

            NoArgSignal signal = injectionBinder.GetInstance<NoArgSignal>() as NoArgSignal;
            signal.Dispatch();
            Assert.AreEqual(testModel.StoredValue, 1);
        }


        [Test]
        public void TestOnce()
        {
            
            commandBinder.Bind<NoArgSignal>().To<NoArgSignalCommand>().Once();

            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreEqual(testModel.StoredValue, 0);


            NoArgSignal signal = injectionBinder.GetInstance<NoArgSignal>() as NoArgSignal;
            signal.Dispatch();
            Assert.AreEqual(testModel.StoredValue, 1);

            signal.Dispatch();
            Assert.AreEqual(testModel.StoredValue, 1); //Should do nothing

        }

        [Test]
        public void TestMultiple()
        {
            commandBinder.Bind<NoArgSignal>().To<NoArgSignalCommand>().To<NoArgSignalCommandTwo>();

            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreEqual(0,testModel.StoredValue);
            NoArgSignal signal = injectionBinder.GetInstance<NoArgSignal>() as NoArgSignal;

            signal.Dispatch();
            Assert.AreEqual(1, testModel.StoredValue); //first command gives 1, second gives 2
            Assert.AreEqual(2, testModel.SecondaryValue); //first command gives 1, second gives 2
        }


        [Test]
        public void TestOneArg()
        {
            commandBinder.Bind<OneArgSignal>().To<OneArgSignalCommand>();

            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreEqual(0, testModel.StoredValue);
            OneArgSignal signal = injectionBinder.GetInstance<OneArgSignal>() as OneArgSignal;

            int injectedValue = 100;
            signal.Dispatch(injectedValue);
            Assert.AreEqual(injectedValue, testModel.StoredValue);
        }

        [Test]
        public void TestTwoArgs()
        {
            commandBinder.Bind<TwoArgSignal>().To<TwoArgSignalCommand>();

            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreEqual(0, testModel.StoredValue);
            TwoArgSignal signal = injectionBinder.GetInstance<TwoArgSignal>() as TwoArgSignal;

            int injectedValue = 100;
            bool injectedBool = true;
            signal.Dispatch(injectedValue, injectedBool); //true should be adding
            Assert.AreEqual(injectedValue, testModel.StoredValue); 

            injectedBool = false;
            signal.Dispatch(injectedValue, injectedBool); //false should be subtracting
            Assert.AreEqual(0, testModel.StoredValue); //first command gives 1, second gives 2
        }

        [Test]
        public void TestTwoArgsSameType()
        {
            commandBinder.Bind<TwoArgSameTypeSignal>().To<TwoArgSameTypeSignalCommand>();

            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreEqual(0, testModel.StoredValue);
            TwoArgSameTypeSignal signal = injectionBinder.GetInstance<TwoArgSameTypeSignal>() as TwoArgSameTypeSignal;

            int injectedValue = 100;
            int secondInjectedValue = 200;
            TestDelegate testDelegate = delegate
            {
                signal.Dispatch(injectedValue, secondInjectedValue); 
            };
            SignalException ex = Assert.Throws<SignalException>(testDelegate);
            Assert.AreEqual(ex.type, SignalExceptionType.COMMAND_VALUE_CONFLICT);
        }


        [Test]
        public void TestSequence()
        {
            //CommandWithInjection requires an ISimpleInterface
            injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToSingleton();

            //Bind the trigger to the command
            commandBinder.Bind<NoArgSignal>().To<CommandWithInjection>().To<CommandWithExecute>().To<CommandWithoutExecute>().InSequence();

            TestDelegate testDelegate = delegate
            {
                NoArgSignal signal = injectionBinder.GetInstance<NoArgSignal>() as NoArgSignal;
                signal.Dispatch();
            };

            //That the exception is thrown demonstrates that the last command ran
            CommandException ex = Assert.Throws<CommandException>(testDelegate);
            Assert.AreEqual(ex.type, CommandExceptionType.EXECUTE_OVERRIDE);

            //That the value is 100 demonstrates that the first command ran
            ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
            Assert.AreEqual(100, instance.intValue);
        }


        [Test]
        public void TestSequenceTwo()
        {
            //Slightly different test to be thorough

            //Bind the trigger to the command
            commandBinder.Bind<TwoArgSignal>().To<TwoArgSignalCommand>().To<TwoArgSignalCommandTwo>().To<TwoArgSignalCommandThree>().InSequence();
            TestModel testModel = injectionBinder.GetInstance<TestModel>() as TestModel;
            int intValue = 1;
            bool boolValue = true;

            TestDelegate testDelegate = delegate
            {
                TwoArgSignal signal = injectionBinder.GetInstance<TwoArgSignal>() as TwoArgSignal;
                signal.Dispatch(intValue, boolValue);
            };

            Assert.Throws<TestPassedException>(testDelegate);
            int intendedValue = 2; //intValue twice (addition because of bool == true)
            Assert.AreEqual(intendedValue, testModel.StoredValue);
        }

        [Test]
        public void TestInterruptedSequence()
        {
            //CommandWithInjection requires an ISimpleInterface
            injectionBinder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToSingleton();

            //Bind the trigger to the command
            commandBinder.Bind <NoArgSignal>().To<CommandWithInjection>().To<FailCommand>().To<CommandWithoutExecute>().InSequence();

            TestDelegate testDelegate = delegate
            {
                NoArgSignal signal = injectionBinder.GetInstance<NoArgSignal>() as NoArgSignal;
                signal.Dispatch();
            };

            //That the exception is not thrown demonstrates that the last command was interrupted
            Assert.DoesNotThrow(testDelegate);

            //That the value is 100 demonstrates that the first command ran
            ISimpleInterface instance = injectionBinder.GetInstance<ISimpleInterface>() as ISimpleInterface;
            Assert.AreEqual(100, instance.intValue);
        }


        class TestModel
        {
            public int StoredValue = 0;
            public int SecondaryValue = 0;
        }

        class NoArgSignal : Signal { }
        class OneArgSignal : Signal<int> { }
        class TwoArgSignal : Signal<int, bool> { }
        class TwoArgSameTypeSignal : Signal<int, int> { }


        class NoArgSignalCommand: Command
        {
            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                TestModel.StoredValue++;
            }
        }

        class NoArgSignalCommandTwo : Command
        {
            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                TestModel.SecondaryValue += 2;
            }
        }

        class OneArgSignalCommand : Command
        {
            [Inject]
            public int injectedValue { get; set; }

            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                TestModel.StoredValue += injectedValue;
            }
        }

        class TwoArgSignalCommand : Command
        {
            [Inject]
            public int injectedValue { get; set; }
            [Inject]
            public bool injectedBool { get; set; }
            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                if (injectedBool)
                    TestModel.StoredValue += injectedValue;
                else
                    TestModel.StoredValue -= injectedValue;
            }
        }

        class TwoArgSignalCommandTwo : Command
        {
            [Inject]
            public int injectedValue { get; set; }
            [Inject]
            public bool injectedBool { get; set; }
            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                if (injectedBool)
                    TestModel.StoredValue += injectedValue;
                else
                    TestModel.StoredValue -= injectedValue;
            }
        }

        class TwoArgSignalCommandThree : Command
        {
            [Inject]
            public int injectedValue { get; set; }
            [Inject]
            public bool injectedBool { get; set; }
            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                throw new TestPassedException("Test Passed");
            }
        }

        class TwoArgSameTypeSignalCommand : Command
        {
            [Inject]
            public int injectedValue { get; set; }
            [Inject]
            public int secondInjectedValue { get; set; }
            [Inject]
            public TestModel TestModel { get; set; }

            public override void Execute()
            {
                //This should never be run
                throw new Exception("This should not be reached");
            }
        }

        class TestPassedException : Exception
        {
            public TestPassedException(string str) : base(str) { }
        }
	}
}
