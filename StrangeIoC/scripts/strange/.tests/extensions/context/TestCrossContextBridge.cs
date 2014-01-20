using NUnit.Framework;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.context.api;
using System;

namespace strange.unittests
{
	public class TestCrossContextBridge
	{
		object view;
		CrossContext Parent;
		CrossContext ChildOne;
		CrossContext ChildTwo;

		private int testValue;

		[SetUp]
		public void SetUp()
		{
			testValue = 0;

			Context.firstContext = null;
			view = new object();
			Parent = new CrossContextTestClass(view, true);
			Parent.Start ();

			ChildOne = new CrossContextTestClass(view, true);
			ChildOne.Start ();

			ChildTwo = new CrossContextTestClass(view, true);
			ChildTwo.Start ();
		}

		[Test]
		public void TestBridgeMapping()
		{
			Assert.IsNotNull (Parent.crossContextBridge);
			Assert.IsNotNull (ChildOne.crossContextBridge);
			Assert.IsNotNull (ChildTwo.crossContextBridge);
			
			Assert.AreSame (Parent.crossContextBridge, ChildOne.crossContextBridge);
			Assert.AreSame (ChildOne.crossContextBridge, ChildTwo.crossContextBridge);
			Assert.AreSame (ChildTwo.crossContextBridge, Parent.crossContextBridge);
		}

		[Test]
		public void TestBridgeParentToChild()
		{
			Parent.crossContextBridge.Bind (SomeEnum.ONE);
			IEventDispatcher parentDispatcher = Parent.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

			IEventDispatcher childDispatcher = ChildOne.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			childDispatcher.AddListener (SomeEnum.ONE, testCallback);

			int sentValue1 = 42;
			int sentValue2 = 43;

			parentDispatcher.Dispatch (SomeEnum.ONE, sentValue1);
			Assert.AreEqual (sentValue1, testValue);

			Parent.crossContextBridge.Unbind (SomeEnum.ONE);

			parentDispatcher.Dispatch (SomeEnum.ONE, sentValue2);
			Assert.AreEqual (sentValue1, testValue);	//didn't change

			//Unit-test wise, this is a bit of a cheat, but it assures me that
			//all Events are returned to the EventDispatcher pool
			Assert.AreEqual (0, EventDispatcher.eventPool.instanceCount - EventDispatcher.eventPool.available);
		}

		[Test]
		public void TestBridgeChildToParent()
		{
			ChildOne.crossContextBridge.Bind (SomeEnum.ONE);
			IEventDispatcher childDispatcher = ChildOne.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

			IEventDispatcher parentDispatcher = Parent.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			parentDispatcher.AddListener (SomeEnum.ONE, testCallback);

			int sentValue1 = 42;
			int sentValue2 = 43;

			childDispatcher.Dispatch (SomeEnum.ONE, sentValue1);
			Assert.AreEqual (sentValue1, testValue);

			ChildOne.crossContextBridge.Unbind (SomeEnum.ONE);

			childDispatcher.Dispatch (SomeEnum.ONE, sentValue2);
			Assert.AreEqual (sentValue1, testValue);

			//Unit-test wise, this is a bit of a cheat, but it assures me that
			//all Events are returned to the EventDispatcher pool
			Assert.AreEqual (0, EventDispatcher.eventPool.instanceCount - EventDispatcher.eventPool.available);
		}

		[Test]
		public void TestBridgeChildToChild()
		{
			ChildTwo.crossContextBridge.Bind (SomeEnum.ONE);	//Note: binding in one Context...
			IEventDispatcher childOneDispatcher = ChildOne.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

			IEventDispatcher childTwoDispatcher = ChildTwo.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			childTwoDispatcher.AddListener (SomeEnum.ONE, testCallback);

			int sentValue1 = 42;
			int sentValue2 = 43;

			childOneDispatcher.Dispatch (SomeEnum.ONE, sentValue1);
			Assert.AreEqual (sentValue1, testValue);

			ChildOne.crossContextBridge.Unbind (SomeEnum.ONE);	//...unbinding in another

			childOneDispatcher.Dispatch (SomeEnum.ONE, sentValue2);
			Assert.AreEqual (sentValue1, testValue);

			//Unit-test wise, this is a bit of a cheat, but it assures me that
			//all Events are returned to the EventDispatcher pool
			Assert.AreEqual (0, EventDispatcher.eventPool.instanceCount - EventDispatcher.eventPool.available);
		}

		private void testCallback(IEvent evt)
		{
			testValue = (int) evt.data;
		}
	}

	class CrossContextTestClass : CrossContext
	{
		public CrossContextTestClass() : base()
		{}

		public CrossContextTestClass(object view, bool autoStartup) : base(view, autoStartup)
		{
		}

		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().ToSingleton().ToName(ContextKeys.CONTEXT_DISPATCHER);
		}
	}

}

