
using NUnit.Framework;
using strange.extensions.context.impl;
using System;

namespace strange.unittests
{
	[TestFixture]
	public class TestContextRemoval
	{
        object view;
        CrossContext Parent;
		TestCrossContextSubclass Child;

        [SetUp]
        public void SetUp()
        {
            Context.firstContext = null;
            view = new object();
            Parent = new CrossContext(view, true);
			Child = new TestCrossContextSubclass(view, true);
        }

		[Test]
		public void TestRemoval()
		{
            Parent.AddContext(Child);

            TestDelegate testDelegate = delegate
            {
                Parent.RemoveContext(Child);
            };

            Assert.Throws<TestPassedException>(testDelegate);
		}
	}


	public class TestCrossContextSubclass : CrossContext
    {
		public TestCrossContextSubclass() : base()
	    {}

		public TestCrossContextSubclass(object view, bool autoStartup) : base(view, autoStartup)
	    {}
         
        public override void OnRemove()
        {
            base.OnRemove();

            throw new TestPassedException("Test Passed");
        }
    }

    class TestPassedException : Exception
    {
        public TestPassedException(string str) : base(str) { }
    }
}
