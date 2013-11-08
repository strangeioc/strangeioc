using NUnit.Framework;
using strange.extensions.context.impl;
using strange.extensions.context.api;
using UnityEngine;
using strange.extensions.mediation.api;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.unittests.annotated.testImplBy;
using strange.unittests.annotated.testDefaultImpl;
using strange.unittests.annotated.testConcrete;
using strange.unittests.annotated.testCrossContext;
using strange.unittests.annotated.testCrossContextInterface;
using System;

namespace strange.unittests
{
    [TestFixture]
	public class TestImplicitBinding
	{

        private MockContext context;
        private object contextView;

        [SetUp]
        public void setup()
        {
            Context.firstContext = null;
            contextView = new object();
            context = new MockContext(contextView, true);
        }

        /// <summary>
        /// Tests our DefaultImpl default case, which is a concrete singleton binding
        /// </summary>
		[Test]
		public void TestDefaultImplConcrete()
		{

            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testConcrete"
            };
            context.Start();

            TestConcreteClass testConcreteClass = context.injectionBinder.GetInstance<TestConcreteClass>() as TestConcreteClass;
            Assert.IsNotNull(testConcreteClass);
             
		}

        [Test]
        public void TestDefaultImplToInterface()
        {
            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testDefaultImpl"
            };
            context.Start();

            TestInterface testImpl = context.injectionBinder.GetInstance<TestInterface>() as TestInterface;
            Assert.IsNotNull(testImpl);
            Assert.IsTrue(typeof(TestInterface).IsAssignableFrom(testImpl.GetType())); //Check that this objects type implements test interface.
            Assert.AreEqual(testImpl.GetType(),typeof(TestImpl)); //Check that its the type we added below
        }


        [Test]
        public void TestImplementedBy()
        {
            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testImplBy" //Namespace is the only true difference. Same tests as above for the same action done by a different method
            };
            context.Start();

            TestInterface testImpl = context.injectionBinder.GetInstance<TestInterface>() as TestInterface;
            Assert.IsNotNull(testImpl);
            Assert.IsTrue(typeof(TestInterface).IsAssignableFrom(testImpl.GetType())); //Check that this objects type implements test interface.
            Assert.AreEqual(testImpl.GetType(), typeof(TestImpl)); //Check that its the type we added below
        }


        /// <summary>
        /// Attempt to bind the interface and the implementation with annotations, which throws an exception
        /// </summary>
        [Test]
        public void TestBindingExistsOne()
        {
            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testImplBy", 
                "strange.unittests.annotated.testDefaultImpl",
            };
            
            TestDelegate testDelegate = delegate
            {
                context.Start();
            };

            //We should be getting an exception here from binding twice
            InjectionException ex = Assert.Throws<InjectionException>(testDelegate);

            //make sure it's the right exception
            Assert.AreEqual(ex.type, InjectionExceptionType.IMPLICIT_BINDING_ALREADY_EXISTS);
        }

        /// <summary>
        /// Attempt to bind two classes implicitly to the same interface. Throws exception
        /// </summary>
        [Test]
        public void TestBindingExistsTwo()
        {
            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testDefaultImpl",
                "strange.unittests.annotated.testBindingAlreadyExists"
            };

            TestDelegate testDelegate = delegate
            {
                context.Start();
            };

            //We should be getting an exception here from binding twice
            InjectionException ex = Assert.Throws<InjectionException>(testDelegate);

            //make sure it's the right exception
            Assert.AreEqual(ex.type, InjectionExceptionType.IMPLICIT_BINDING_ALREADY_EXISTS);
        }

        /// <summary>
        /// Attempt to bind an ImplementedBy annotation pointing to a Type which does not implement the interface
        /// </summary>
        [Test]
        public void TestDoesNotImplement()
        {
            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testDoesntImplement",
            };

            TestDelegate testDelegate = delegate
            {
                context.Start();
            };

            //We should be getting an exception here from binding twice
            InjectionException ex = Assert.Throws<InjectionException>(testDelegate);

            //make sure it's the right exception
            Assert.AreEqual(ex.type, InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT);
        }

        /// <summary>
        /// Attempt to bind an ImplementedBy annotation pointing to a Type which does not implement the interface
        /// </summary>
        [Test]
        public void TestDoesNotImplementTwo()
        {
            context.ScannedPackages = new string[]{
                "strange.unittests.annotated.testDoesntImplementTwo",
            };

            TestDelegate testDelegate = delegate
            {
                context.Start();
            };

            //We should be getting an exception here from binding twice
            InjectionException ex = Assert.Throws<InjectionException>(testDelegate);

            //make sure it's the right exception
            Assert.AreEqual(ex.type, InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT);
        }

        /// <summary>
        /// Test [CrossContextImpl] tag. 
        /// This is not meant to be a test of all crosscontext functionality, just the tag
        /// The CrossContext tag really just tells a binding to call .CrossContext()
        /// See TestCrossContext for tests of CrossContext
        /// </summary>
        [Test]
        public void TestCrossContextImplicit()
        {
            object viewParent = new object();
            object viewChildOne  = new object();
            object viewChildTwo = new object();
            MockContext Parent = new MockContext(viewParent, true);
            MockContext ChildOne = new MockContext(viewChildOne, true); //Ctr will automatically add to Context.firstcontext. No need to call it manually (and you should not).
            MockContext ChildTwo = new MockContext(viewChildTwo, true);


            Parent.ScannedPackages = new string[]{
                "strange.unittests.annotated.testCrossContext"
            };

            Parent.Start();
            ChildOne.Start();
            ChildTwo.Start();

            TestCrossContextInterface parentModel = Parent.injectionBinder.GetInstance<TestCrossContextInterface>() as TestCrossContextInterface;

            TestCrossContextInterface childOneModel = ChildOne.injectionBinder.GetInstance<TestCrossContextInterface>() as TestCrossContextInterface;
            Assert.IsNotNull(childOneModel);
            TestCrossContextInterface childTwoModel = ChildTwo.injectionBinder.GetInstance<TestCrossContextInterface>() as TestCrossContextInterface;
            Assert.IsNotNull(childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel); //These two should be the same object

            Assert.AreEqual(0, parentModel.Value); //start at 0, might as well verify.

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value); //child one is updated

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value); //child two is updated
        }


        /// <summary>
        /// Test [CrossContextImpl] tag. 
        /// Child contexts should be able to 'override' CrossContext bindings with local bindings
        /// </summary>
        [Test]
        public void TestCrossContextAllowsOverrides()
        {
            object viewParent = new object();
            object viewChildOne = new object();
            object viewChildTwo = new object();
            MockContext Parent = new MockContext(viewParent, true);
            MockContext ChildOne = new MockContext(viewChildOne, true); //Ctr will automatically add to Context.firstcontext. No need to call it manually (and you should not).
            MockContext ChildTwo = new MockContext(viewChildTwo, true);


            Parent.ScannedPackages = new string[]{
                "strange.unittests.annotated.testCrossContext"
            };

            ChildOne.ScannedPackages = new string[]{
                "strange.unittests.annotated.testCrossOverride"
            };
            Parent.Start();
            ChildOne.Start();
            ChildTwo.Start();

            TestCrossContextInterface parentModel = Parent.injectionBinder.GetInstance<TestCrossContextInterface>() as TestCrossContextInterface; 
            //Get the instance from the parent injector (The cross context binding)

            TestCrossContextInterface childOneModel = ChildOne.injectionBinder.GetInstance<TestCrossContextInterface>() as TestCrossContextInterface;
            Assert.AreNotSame(childOneModel, parentModel); //The value from getinstance is NOT the same as the cross context value. We have overidden the cross context value locally

            TestCrossContextInterface childTwoModel = ChildTwo.injectionBinder.GetInstance<TestCrossContextInterface>() as TestCrossContextInterface;
            Assert.IsNotNull(childTwoModel);
            Assert.AreNotSame(childOneModel, childTwoModel); //These two are different objects, the childTwoModel being cross context, and childone being the override
            Assert.AreSame(parentModel, childTwoModel); //Both cross context models are the same


            parentModel.Value++;
            Assert.AreEqual(1, childTwoModel.Value); //cross context model should be changed

            parentModel.Value++;
            Assert.AreEqual(1000, childOneModel.Value); //local model is not changed


            Assert.AreEqual(2, parentModel.Value); //cross context model is changed
        }

	}

}

namespace strange.unittests.annotated.testConcrete
{
    [DefaultImpl]
    public class TestConcreteClass { }
}

namespace strange.unittests.annotated.testImplBy
{
    [ImplementedBy(typeof(TestImpl))]
    public interface TestInterface { }
}

namespace strange.unittests.annotated.testDefaultImpl
{
    [DefaultImpl(typeof(TestInterface))]
    public class TestImpl : TestInterface { }
}

namespace strange.unittests.annotated.testBindingAlreadyExists
{
    [DefaultImpl(typeof(TestInterface))]
    public class TestImplTwo : TestInterface { }
}

namespace strange.unittests.annotated.testDoesntImplement
{
    [ImplementedBy(typeof(TestClassDoesntImplement))]
    public interface TestInterfaceDoesntImplement { }

    public class TestClassDoesntImplement { }
}

namespace strange.unittests.annotated.testDoesntImplementTwo
{
    [ImplementedBy(typeof(TestClassDoesntImplement))]
    public interface TestInterfaceDoesntImplement { }

    [DefaultImpl(typeof(TestInterfaceDoesntImplement))]
    public class TestClassDoesntImplement { }
}

namespace strange.unittests.annotated.testCrossContextInterface
{
    public interface TestCrossContextInterface 
    {
        int Value { get; set; }
    }
}
namespace strange.unittests.annotated.testCrossContext
{
    [DefaultImpl(typeof(TestCrossContextInterface))]
    [CrossContextComponent]
    public class TestConcreteCrossContextClass : TestCrossContextInterface
    {
        public TestConcreteCrossContextClass()
        {
            Value = 0;
        }
        public int Value { get; set; }
    } 
}

namespace strange.unittests.annotated.testCrossOverride
{
    [DefaultImpl(typeof(TestCrossContextInterface))]
    public class TestConcreteCrossContextClassOverride : TestCrossContextInterface
    {
        public TestConcreteCrossContextClassOverride()
        {
            Value = 1000;
        }
        public int Value { get; set; }
    }
}
