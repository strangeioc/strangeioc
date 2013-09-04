using NUnit.Framework;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using System;
using strange.extensions.context.impl;
using System.Collections;
using UnityEngine;

namespace strange.unittests
{

    /**
     * Some functionality for Cross Context will be impossible to test
     * The goal here is to test whatever we can, but the actual implementation will 
     * vary per users and rely heavily on Unity in most cases, which we can't test here.
     **/ 
	[TestFixture]
	public class TestCrossContext
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
            Parent = new CrossContext(view, true);
            ChildOne = new CrossContext(view, true);
            ChildTwo = new CrossContext(view, true);
        }
        
        [Test]
        public void TestCrossInjectionFromValue()
        {
            TestModel parentModel = new TestModel();
            Parent.injectionBinder.Bind<TestModel>().ToValue(parentModel).CrossContext(); //bind it once here and it should be accessible everywhere

            TestModel parentModelTwo = Parent.injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreSame(parentModel, parentModelTwo); //Assure that this value is correct

            TestModel childOneModel = ChildOne.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childOneModel);
            TestModel childTwoModel = ChildTwo.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel); //These two should be the same object

            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value);

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value);

        }


        [Test]
        public void TestCrossInjectionFromSingleton()
        {
            Parent.injectionBinder.Bind<TestModel>().ToSingleton().CrossContext(); //bind it once here and it should be accessible everywhere

            TestModel parentModel = Parent.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(parentModel);

            TestModel childOneModel = ChildOne.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childOneModel);
            TestModel childTwoModel = ChildTwo.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childTwoModel);
            
            Assert.AreSame(parentModel, childOneModel); 
            Assert.AreSame(parentModel, childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel);


            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value);

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value);

        }


	}


    public class TestModel
    {
        public int Value = 0;
    }

}
