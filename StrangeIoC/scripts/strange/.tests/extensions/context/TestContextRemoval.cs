
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
        TestContext Child;

        [SetUp]
        public void SetUp()
        {
            Context.firstContext = null;
            view = new object();
            Parent = new CrossContext(view, true);
            Child = new TestContext(view, true);
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


    public class TestContext : CrossContext
    {
        public TestContext() : base()
	    {}

        public TestContext(object view, bool autoStartup) : base(view, autoStartup)
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
