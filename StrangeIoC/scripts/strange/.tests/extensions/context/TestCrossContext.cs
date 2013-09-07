using NUnit.Framework;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;

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
        public void TestValue()
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
        public void TestFactory()
        {
            TestModel parentModel = new TestModel();
            Parent.injectionBinder.Bind<TestModel>().To<TestModel>().CrossContext();

            TestModel parentModelTwo = Parent.injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreNotSame(parentModel, parentModelTwo); //As it's a factory, we should not have the same objects

            TestModel childOneModel = ChildOne.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childOneModel);
            TestModel childTwoModel = ChildTwo.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childTwoModel);
            Assert.AreNotSame(childOneModel, childTwoModel); //These two should be DIFFERENT

            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(0, childOneModel.Value); //doesn't change

            parentModel.Value++;
            Assert.AreEqual(0, childTwoModel.Value); //doesn't change

        }

        [Test]
        public void TestNamed()
        {
            string name = "Name";
            TestModel parentModel = new TestModel();
            Parent.injectionBinder.Bind<TestModel>().ToValue(parentModel).ToName(name).CrossContext(); //bind it once here and it should be accessible everywhere

            TestModel parentModelTwo = Parent.injectionBinder.GetInstance<TestModel>(name) as TestModel;

            Assert.AreSame(parentModel, parentModelTwo); //Assure that this value is correct

            TestModel childOneModel = ChildOne.injectionBinder.GetInstance<TestModel>(name) as TestModel;
            Assert.IsNotNull(childOneModel);
            TestModel childTwoModel = ChildTwo.injectionBinder.GetInstance<TestModel>(name) as TestModel;
            Assert.IsNotNull(childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel); //These two should be the same object

            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value);

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value);
        }

        //test that local bindings will override cross bindings
        [Test]
        public void TestLocalOverridesCrossContext()
        {
            Parent.injectionBinder.Bind<TestModel>().ToSingleton().CrossContext(); //bind the cross context binding.
            TestModel initialChildOneModel = new TestModel();
            initialChildOneModel.Value = 1000;


            ChildOne.injectionBinder.Bind<TestModel>().ToValue(initialChildOneModel); //Bind a local override in this child

            TestModel parentModel = Parent.injectionBinder.GetInstance<TestModel>() as TestModel; //Get the instance from the parent injector (The cross context binding)


            TestModel childOneModel = ChildOne.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.AreSame(initialChildOneModel, childOneModel); // The value from getInstance is the same as the value we just mapped as a value locally
            Assert.AreNotSame(childOneModel, parentModel); //The value from getinstance is NOT the same as the cross context value. We have overidden the cross context value locally


            TestModel childTwoModel = ChildTwo.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childTwoModel);
            Assert.AreNotSame(childOneModel, childTwoModel); //These two are different objects, the childTwoModel being cross context, and childone being the override
            Assert.AreSame(parentModel, childTwoModel); //Both cross context models are the same


            parentModel.Value++;
            Assert.AreEqual(1, childTwoModel.Value); //cross context model should be changed

            parentModel.Value++;
            Assert.AreEqual(1000, childOneModel.Value); //local model is not changed


            Assert.AreEqual(2, parentModel.Value); //cross context model is changed

        }

        [Test]
        public void TestSingleton()
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

            IInjectionBinding binding = Parent.injectionBinder.GetBinding<TestModel>();
            Assert.IsNotNull(binding);
            Assert.IsTrue(binding.isCrossContext);

            IInjectionBinding childBinding = ChildOne.injectionBinder.GetBinding<TestModel>();
            Assert.IsNotNull(childBinding);
            Assert.IsTrue(childBinding.isCrossContext);


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
