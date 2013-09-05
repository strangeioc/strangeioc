using NUnit.Framework;
using strange.extensions.context.impl;

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
            Parent.injectionBinder.Bind<TestModel>().ToSingleton().CrossContext(); //bind it once here and it should be accessible everywhere
            TestModel parentModel = new TestModel();
            parentModel.Value = 1000;
            //Parent.injectionBinder.Unbind<TestModel>();
            Parent.injectionBinder.Bind<TestModel>().ToValue(parentModel);

            TestModel parentModelTwo = Parent.injectionBinder.GetInstance<TestModel>() as TestModel;

            Assert.AreSame(parentModel, parentModelTwo); //Assure that this value is the same as the locally injected alue


            TestModel childOneModel = ChildOne.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childOneModel);
            TestModel childTwoModel = ChildTwo.injectionBinder.GetInstance<TestModel>() as TestModel;
            Assert.IsNotNull(childTwoModel);

            Assert.AreSame(childOneModel, childTwoModel); //These two should be the same object, both through the cross context and both different from the parent model
            Assert.AreNotSame(parentModel, childOneModel); 
            Assert.AreNotSame(parentModel, childTwoModel);


            parentModel.Value++;
            Assert.AreEqual(0, childOneModel.Value); //doesn't change

            parentModel.Value++;
            Assert.AreEqual(0, childTwoModel.Value); //doesn't cahnge


            Assert.AreEqual(1002, parentModel.Value); //this was unchanged

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
