using NUnit.Framework;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.unittests.annotated.multipleInterfaces;
using strange.unittests.annotated.namespaceTest.one;
using strange.unittests.annotated.namespaceTest.three.even.farther;
using strange.unittests.annotated.namespaceTest.two.far;
using strange.unittests.annotated.testConcreteNamed;
using strange.unittests.annotated.testImplBy;
using strange.unittests.annotated.testImplements;
using strange.unittests.annotated.testConcrete;
using strange.unittests.annotated.testCrossContextInterface;
using strange.unittests.annotated.testImplTwo;
using strange.unittests.testimplicitbindingnamespace;
using System.Collections.Generic;

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
		/// Tests our Implements default case, which is a concrete singleton binding
		/// </summary>
		[Test]
		public void TestImplementsConcrete()
		{

			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.testConcrete"
			};
			context.Start();

			TestConcreteClass testConcreteClass = context.injectionBinder.GetInstance<TestConcreteClass>() as TestConcreteClass;
			Assert.IsNotNull(testConcreteClass);
			 
		}

		[Test]
		public void TestImplementsNamedConcrete()
		{
			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.testConcreteNamed"
			};
			context.Start();

			TestConcreteNamedClass testConcreteClass = context.injectionBinder.GetInstance<TestConcreteNamedClass>("NAME") as TestConcreteNamedClass;
			Assert.IsNotNull(testConcreteClass);
		}

		/// <summary>
		/// Test binding a concrete class to an interface using the Implements tag
		/// </summary>
		[Test]
		public void TestImplementsToInterface()
		{
			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.testImplements"
			};
			context.Start();

			TestInterface testImpl = context.injectionBinder.GetInstance<TestInterface>() as TestInterface;
			Assert.IsNotNull(testImpl);
			Assert.IsTrue(typeof(TestInterface).IsAssignableFrom(testImpl.GetType())); //Check that this objects type implements test interface.
			Assert.AreEqual(testImpl.GetType(),typeof(TestImpl)); //Check that its the type we added below
		}


		/// <summary>
		/// Test binding a default concrete class to an interface using the ImplementedBy tag (on the interface)
		/// </summary>
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
		/// Bind via an ImplementedBy tag, followed by an Implements from a different class.
		/// Implements should override the ImplementedBy tag
		/// </summary>
		[Test]
		public void TestImplementsOverridesImplementedBy()
		{
			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.testImplTwo", 
				"strange.unittests.annotated.testImplBy",
			};

			context.Start();


			TestInterface testInterface = context.injectionBinder.GetInstance<TestInterface>() as TestInterface;
			Assert.True(testInterface is TestImplTwo);
		}

		/// <summary>
		/// Bind implicitly and then overwrite with an explicit binding
		/// </summary>
		[Test]
		public void TestExplicitBindingOverrides()
		{
			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.testImplements",
			};

			context.Start();


			TestInterface testInterfacePre = context.injectionBinder.GetInstance<TestInterface>() as TestInterface;
			Assert.True(testInterfacePre is TestImpl);
			//Confirm the previous binding is the implicit binding as expected

			context.injectionBinder.Bind<TestInterface>().To<TestImplTwo>();
			TestInterface testInterfacePost = context.injectionBinder.GetInstance<TestInterface>() as TestInterface;
			Assert.True(testInterfacePost is TestImplTwo);
			//Confirm the new binding is the one we just wrote
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

			//We should be getting an exception here because the interface is not implemented
			InjectionException ex = Assert.Throws<InjectionException>(testDelegate);

			//make sure it's the right exception
			Assert.AreEqual(ex.type, InjectionExceptionType.IMPLICIT_BINDING_IMPLEMENTOR_DOES_NOT_IMPLEMENT_INTERFACE);
		}


		/// <summary>
		/// Attempt to bind an Implements annotation pointing to an interface it does not implement
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

			//We should be getting an exception here because the interface is not implemented
			InjectionException ex = Assert.Throws<InjectionException>(testDelegate);

			//make sure it's the right exception
			Assert.AreEqual(ex.type, InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT_DESIGNATED_INTERFACE);
		}

		/// <summary>
		/// Test [CrossContextComponent] tag. 
		/// This is not meant to be a test of all crosscontext functionality, just the tag
		/// The CrossContextComponent tag really just tells a binding to call .CrossContext()
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
		/// Test [CrossContextComponent] tag. 
		/// Child contexts should be able to 'override' Cross-Context bindings with local bindings
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

		//This monster "unit" test confirms that implicit bindings
		//correctly maintain integrity across Context boundaries.
		[Test]
		public void TestMultipleCrossContextImplicitBindings()
		{
			TestImplicitBindingClass.instantiationCount = 0;

			int contextsBeforeInstancing = 3;
			int contextsAfterInstancing = 4;
			int contextsToCreate = contextsBeforeInstancing + contextsAfterInstancing;
			int getInstanceCallsPerContext = 3;
			int injectsPerContext = 3;
			List<Context> contexts = new List<Context>();
			IInjectionBinding bindingBeforeContextCreation = null;
			object bindingValueBeforeContextCreation = null;

			//We create several Contexts.
			//note contextNumber is 1-based
			for (int contextNumber = 1; contextNumber <= contextsToCreate; contextNumber++)
			{
				//The first batch of Contexts don't actually create instances, just the implicit bindings
				bool toInstance = (contextNumber > contextsBeforeInstancing);
				//Specifically call out the Context that is first to create actual instances
				bool isFirstContextToCallGetInstance = (contextNumber == (contextsBeforeInstancing + 1));

				//Create each "ContextView" and its Context
				object mockGameObject = new object ();
				TestImplicitBindingContext context = new TestImplicitBindingContext (mockGameObject);
				contexts.Add (context);

				//For each Context, check that the TestImplicitBindingClass BINDING exists (no instance created yet)
				IInjectionBinding bindingAfterContextCreation = context.injectionBinder.GetBinding<TestImplicitBindingClass> ();
				object bindingValueAfterContextCreation = bindingAfterContextCreation.value;
				
				bool bindingChangedDueToContextCreation = bindingAfterContextCreation != bindingBeforeContextCreation;
				bool bindingValueChangedDueToContextCreation = bindingValueAfterContextCreation != bindingValueBeforeContextCreation;

				//due to the weak binding replacement rules, the binding should change every time we scan until we instance
				Assert.IsFalse (bindingChangedDueToContextCreation && toInstance && !isFirstContextToCallGetInstance);

				//after creating a new context, the value of the binding should only change on the first context
				//(it was null before that)
				Assert.IsFalse (bindingValueChangedDueToContextCreation && contextNumber != 1);

				if (toInstance)
				{
					//For the Contexts that actually create instances...
					for (int a = 0; a < getInstanceCallsPerContext; a++)
					{
						//...create some instances (well, duh) of the TestImplicitBindingClass...
						TestImplicitBindingClass instance = context.injectionBinder.GetInstance<TestImplicitBindingClass> ();
						Assert.IsNotNull (instance);
					}

					for (int b = 0; b < injectsPerContext; b++)
					{
						//...and some instances of the class that gets injected with TestImplicitBindingClass.
						TestImplicitBindingInjectionReceiver instance = context.injectionBinder.GetInstance<TestImplicitBindingInjectionReceiver> ();
						Assert.IsNotNull (instance);
						Assert.IsNotNull (instance.testImplicitBindingClass);
					}
				}

				//We inspect the binding and its value after all this mapping/instantiation
				IInjectionBinding bindingAfterGetInstanceCalls = context.injectionBinder.GetBinding<TestImplicitBindingClass> ();
				object bindingValueAfterGetInstanceCalls = bindingAfterGetInstanceCalls.value;
				
				bool bindingChangedDueToGetInstanceCalls = bindingAfterGetInstanceCalls != bindingAfterContextCreation;
				bool bindingValueChangedDueToGetInstanceCalls = bindingValueAfterGetInstanceCalls != bindingValueAfterContextCreation;

				//the binding itself should only change during the scan
				Assert.IsFalse (bindingChangedDueToGetInstanceCalls);

				//if the weak binding replacement rules are working, the only time the value should
				//change is the first time we call GetInstance
				Assert.IsFalse (bindingValueChangedDueToGetInstanceCalls && !isFirstContextToCallGetInstance);

				//reset values for the next pass
				bindingBeforeContextCreation = bindingAfterGetInstanceCalls;
				bindingValueBeforeContextCreation = bindingValueAfterGetInstanceCalls;
			}

			//This is a Cross-Context Singleton.
			//The primary purpose of this test is to ensure (that under the circumstances of this test),
			//TestImplicitBindingClass should only get instantiated once
			Assert.AreEqual (1, TestImplicitBindingClass.instantiationCount);
		}

		/// <summary>
		/// Test that our assumptions regarding namespace scoping are correct 
		/// (e.g. company.project.feature will include company.project.feature.signal)
		/// </summary>
		[Test]
		public void TestNamespaces()
		{
			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.namespaceTest"
			};
			context.Start();

			//Should bind 3 classes concretely in the 
			TestNamespaceOne one = context.injectionBinder.GetInstance<TestNamespaceOne>() as TestNamespaceOne;
			Assert.NotNull(one);

			TestNamespaceTwo two = context.injectionBinder.GetInstance<TestNamespaceTwo>() as TestNamespaceTwo;
			Assert.NotNull(two);

			TestNamespaceThree three = context.injectionBinder.GetInstance<TestNamespaceThree>() as TestNamespaceThree;
			Assert.NotNull(three);
		}

		[Test]
		public void TestMultipleImplements()
		{
			context.ScannedPackages = new string[]{
				"strange.unittests.annotated.multipleInterfaces"
			};
			context.Start();

			TestInterfaceOne one = context.injectionBinder.GetInstance<TestInterfaceOne>() as TestInterfaceOne;
			Assert.NotNull(one);

			TestInterfaceTwo two = context.injectionBinder.GetInstance<TestInterfaceTwo>() as TestInterfaceTwo;
			Assert.NotNull(two);

			TestInterfaceThree three = context.injectionBinder.GetInstance<TestInterfaceThree>() as TestInterfaceThree;
			Assert.NotNull(three);

			Assert.AreEqual(one, two);
			Assert.AreEqual(one, three);
		}
	}


}

namespace strange.unittests.annotated.testConcrete
{
	[Implements]
	public class TestConcreteClass { }
}

namespace strange.unittests.annotated.testConcreteNamed
{
	[Implements(InjectionBindingScope.SINGLE_CONTEXT, "NAME")]
	public class TestConcreteNamedClass { }
}

namespace strange.unittests.annotated.testImplBy
{
	[ImplementedBy(typeof(TestImpl))]
	public interface TestInterface { }
}

namespace strange.unittests.annotated.testImplements
{
	[Implements(typeof(TestInterface))]
	public class TestImpl : TestInterface { }
}

namespace strange.unittests.annotated.testImplTwo
{
	[Implements(typeof(TestInterface))]
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
	public interface TestInterfaceDoesntImplement { }

	[Implements(typeof(TestInterfaceDoesntImplement))]
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
	[Implements(typeof(TestCrossContextInterface), InjectionBindingScope.CROSS_CONTEXT)]
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
	[Implements(typeof(TestCrossContextInterface))]
	public class TestConcreteCrossContextClassOverride : TestCrossContextInterface
	{
		public TestConcreteCrossContextClassOverride()
		{
			Value = 1000;
		}
		public int Value { get; set; }
	}
}

namespace strange.unittests.annotated.namespaceTest.one
{
	[Implements]
	public class TestNamespaceOne {}
}

namespace strange.unittests.annotated.namespaceTest.two.far
{
	[Implements]
	public class TestNamespaceTwo {}
}

namespace strange.unittests.annotated.namespaceTest.three.even.farther
{
	[Implements]
	public class TestNamespaceThree {}
}

namespace strange.unittests.annotated.multipleInterfaces
{

	public interface TestInterfaceOne {}
	public interface TestInterfaceTwo { }
	public interface TestInterfaceThree { }

	[Implements(typeof(TestInterfaceOne))]
	[Implements(typeof(TestInterfaceTwo))]
	[Implements(typeof(TestInterfaceThree))]
	public class TestMultipleImplementer : TestInterfaceOne, TestInterfaceTwo, TestInterfaceThree
	{
		
	}
	
}

namespace strange.unittests.testimplicitbindingnamespace
{
	public class TestImplicitBindingContext : MockContext 
	{
		public TestImplicitBindingContext(object contextView) : base(contextView){}
		protected override void mapBindings()
		{
			implicitBinder.ScanForAnnotatedClasses(new string[]{"strange.unittests.testimplicitbindingnamespace"});
			injectionBinder.Bind<TestImplicitBindingInjectionReceiver>().ToSingleton();
		}
	}


	public class TestImplicitBindingInjectionReceiver
	{
		[Inject]
		public TestImplicitBindingClass testImplicitBindingClass{get;set;}
	}

	[Implements(InjectionBindingScope.CROSS_CONTEXT)]
	public class TestImplicitBindingClass
	{
		public static int instantiationCount = 0;
		public TestImplicitBindingClass()
		{
			++instantiationCount;
		}
	}
}
