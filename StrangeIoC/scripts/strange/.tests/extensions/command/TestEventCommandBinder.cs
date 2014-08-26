using System;
using NUnit.Framework;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;

namespace strange.unittests
{
    [TestFixture]
    public class TestEventCommandBinder
    {
        private IInjectionBinder injectionBinder;
        private ICommandBinder commandBinder;
        private IEventDispatcher eventDispatcher;

        [SetUp]
        public void SetUp()
        {
            
            injectionBinder = new InjectionBinder();
            injectionBinder.Bind<IInjectionBinder>().Bind<IInstanceProvider>().ToValue(injectionBinder);
            injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().ToSingleton().ToName(ContextKeys.CONTEXT_DISPATCHER);
            injectionBinder.Bind<ICommandBinder>().To<EventCommandBinder>().ToSingleton();
            commandBinder = injectionBinder.GetInstance<ICommandBinder>();
            eventDispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER);
            (eventDispatcher as ITriggerProvider).AddTriggerable(commandBinder as ITriggerable);
            BadCommand.TestValue = 0;

        }

        [TearDown]
        public void TearDown()
        {
            EventDispatcher.eventPool.Clean();
        }

        [Test] public void TestBadConstructorCleanup()
        {
            commandBinder.Bind(TestEvent.TEST).To<BadCommand>();

            Assert.Throws<CommandException>(delegate
            {
                eventDispatcher.Dispatch(TestEvent.TEST);
                Assert.AreEqual(0,BadCommand.TestValue); //execute did not run
            });

            //Run it back, and assert that it throws a CommandException
            //It should *NOT* throw a binder exception
            //Recent fix to wrap createCommand with a try catch finally block fixes this previous bug.
            Assert.Throws<CommandException>(delegate
            {
                eventDispatcher.Dispatch(TestEvent.TEST);
            });

        }

        class TestEvent : IEvent
        {

            public const string TEST = "TEST";
            public object type { get; set; }
            public IEventDispatcher target { get; set; }
            public object data { get; set; }

        }
        class BadCommand : EventCommand
        {
            public static int TestValue;
            public BadCommand()
            {
                //This is nonsense, but should break horribly on GetInstance
                injectionBinder.Bind<IEvent>().To(new TestEvent());
            }
            public override void Execute()
            {
                TestValue++;
            }
        }
    }
}
