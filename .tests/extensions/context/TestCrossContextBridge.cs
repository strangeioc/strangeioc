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

		[SetUp]
		public void SetUp()
		{
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

			TestDelegate testDelegate = delegate() {
				parentDispatcher.Dispatch (SomeEnum.ONE);
			};

			Assert.Throws<CCBTestPassedException> (testDelegate);

			Parent.crossContextBridge.Unbind (SomeEnum.ONE);
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestBridgeChildToParent()
		{
			ChildOne.crossContextBridge.Bind (SomeEnum.ONE);
			IEventDispatcher childDispatcher = ChildOne.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

			IEventDispatcher parentDispatcher = Parent.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			parentDispatcher.AddListener (SomeEnum.ONE, testCallback);

			TestDelegate testDelegate = delegate() {
				childDispatcher.Dispatch (SomeEnum.ONE);
			};

			Assert.Throws<CCBTestPassedException> (testDelegate);

			ChildOne.crossContextBridge.Unbind (SomeEnum.ONE);
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestBridgeChildToChild()
		{
			ChildTwo.crossContextBridge.Bind (SomeEnum.ONE);	//Note: binding in one Context...
			IEventDispatcher childOneDispatcher = ChildOne.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

			IEventDispatcher childTwoDispatcher = ChildTwo.injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			childTwoDispatcher.AddListener (SomeEnum.ONE, testCallback);

			TestDelegate testDelegate = delegate() {
				childOneDispatcher.Dispatch (SomeEnum.ONE);
			};

			Assert.Throws<CCBTestPassedException> (testDelegate);

			ChildOne.crossContextBridge.Unbind (SomeEnum.ONE);	//...unbinding in another
			Assert.DoesNotThrow (testDelegate);
		}

		private void testCallback()
		{
			throw new CCBTestPassedException ("Test Passed");
		}
	}

	class CCBTestPassedException : Exception
	{
		public CCBTestPassedException(string str) : base(str) { }
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

