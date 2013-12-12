using NUnit.Framework;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.extensions.context.api;

namespace strange.unittests
{

	/**
     * Test the startup routine for a basic Context.
     **/ 
	[TestFixture]
	public class TestContext
	{
		object view;

		[SetUp]
		public void SetUp()
		{
			Context.firstContext = null;
			view = new object();
		}

		[Test]
		public void TestContextIsFirstContext()
		{
			Context context = new Context (view);
			Assert.AreEqual (context, Context.firstContext);
		}

		[Test]
		public void TestContextView()
		{
			Context context = new Context (view);
			Assert.AreEqual (view, context.contextView);
		}

		[Test]
		public void TestAutoStartup()
		{
			TestContextSubclass context = new TestContextSubclass (view);
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestInterruptMapping()
		{
			TestContextSubclass context = new TestContextSubclass (view, ContextStartupFlags.MANUAL_MAPPING);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestInterruptLaunch()
		{
			TestContextSubclass context = new TestContextSubclass (view, ContextStartupFlags.MANUAL_LAUNCH);
			Assert.AreEqual (TestContextSubclass.MAPPING_VALUE, context.testValue);
			context.Launch ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestInterruptAll()
		{
			TestContextSubclass context = new TestContextSubclass (view, ContextStartupFlags.MANUAL_MAPPING | ContextStartupFlags.MANUAL_LAUNCH);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.MAPPING_VALUE, context.testValue);
			context.Launch ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestOldStyleInterruptLaunch()
		{
			TestContextSubclass context = new TestContextSubclass (view, false);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.MAPPING_VALUE, context.testValue);
			context.Launch ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}

		[Test]
		public void TestOldStyleAutoStartup()
		{
			TestContextSubclass context = new TestContextSubclass (view, true);
			Assert.AreEqual (TestContextSubclass.INIT_VALUE, context.testValue);
			context.Start ();
			Assert.AreEqual (TestContextSubclass.LAUNCH_VALUE, context.testValue);
		}
	}

	class TestContextSubclass : Context
	{
		public static string INIT_VALUE = "Zaphod";
		public static string MAPPING_VALUE = "Ford Prefect";
		public static string LAUNCH_VALUE = "Arthur Dent";

		private string _testValue = INIT_VALUE;
		public string testValue
		{
			get { return _testValue; }
		}

		public TestContextSubclass (object view) : base(view){}
		public TestContextSubclass (object view, bool autoMapping) : base(view, autoMapping){}
		public TestContextSubclass (object view, ContextStartupFlags flags) : base(view, flags){}


		protected override void mapBindings ()
		{
			base.mapBindings ();
			_testValue = MAPPING_VALUE;
		}

		public override void Launch ()
		{
			base.Launch ();
			_testValue = LAUNCH_VALUE;
		}
	}
}
