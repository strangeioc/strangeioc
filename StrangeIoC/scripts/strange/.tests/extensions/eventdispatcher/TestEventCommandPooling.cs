using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.command.api;
using strange.framework.api;
using strange.extensions.command.impl;
using strange.extensions.injector.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.api;
using strange.extensions.context.api;

namespace strange.unittests
{
	[TestFixture()]
	public class TestEventCommandPooling
	{
		IInjectionBinder injectionBinder;
		ICommandBinder commandBinder;
		IEventDispatcher dispatcher;

		[SetUp]
		public void SetUp()
		{
			CommandThrowsErrorIfEventIsNull.result = 0;
			CommandThrowsErrorIfEventIsNull.timesExecuted = 0;

			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IInjectionBinder> ().Bind<IInstanceProvider> ().ToValue (injectionBinder);
			injectionBinder.Bind<ICommandBinder> ().To<EventCommandBinder> ().ToSingleton ();
			injectionBinder.Bind<IEventDispatcher> ().To<EventDispatcher> ().ToSingleton ().ToName(ContextKeys.CONTEXT_DISPATCHER);

			commandBinder = injectionBinder.GetInstance<ICommandBinder> ();
			dispatcher = injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER);
			(dispatcher as ITriggerProvider).AddTriggerable (commandBinder as ITriggerable);
		}



		[Test]
		public void TestEventPoolingInCommandSequence()
		{
			commandBinder.Bind(SomeEnum.ONE).To<CommandThrowsErrorIfEventIsNull>().To<CommandThrowsErrorIfEventIsNull2>().To<CommandThrowsErrorIfEventIsNull3>().InSequence();
			dispatcher.Dispatch (SomeEnum.ONE, 100);

			Assert.AreEqual (3, CommandThrowsErrorIfEventIsNull.timesExecuted);
			Assert.AreEqual (100 * 2, CommandThrowsErrorIfEventIsNull.result);

			//Events should have been returned to pool
			int itemsDedicated = EventDispatcher.eventPool.instanceCount - EventDispatcher.eventPool.available;

			Assert.AreEqual (0, itemsDedicated);
		}
	}
}

