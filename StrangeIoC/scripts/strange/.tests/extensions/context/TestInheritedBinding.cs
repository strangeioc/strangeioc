using System;
using NUnit.Framework;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;

namespace strange.unittests
{
	[TestFixture]
	public class TestInheritedBinding
	{

		private object view;
		private HierarchicalContext RootContext;
		private HierarchicalContext ParentContext;
		private HierarchicalContext ChildContext;



		[SetUp]
		public void setup()
		{
			view = new object();
			RootContext = new HierarchicalContext(view, ContextStartupFlags.AUTOMATIC);
			ParentContext = new HierarchicalContext(view, ContextStartupFlags.AUTOMATIC | ContextStartupFlags.MANUAL_HIERARCHY);
			ChildContext = new HierarchicalContext(view, ContextStartupFlags.AUTOMATIC | ContextStartupFlags.MANUAL_HIERARCHY);
		}

		[TearDown]
		public void teardown()
		{
			RootContext = null;
			ParentContext = null;
			ChildContext = null;
			Context.firstContext = null;
		}


		[Test]
		public void TestContextAddedBeforeBinding()
		{
			RootContext.AddContext(ParentContext);
			ParentContext.AddContext(ChildContext);

			var valueModel = BindTestModel();

			//parent and child contexts should contain this same value binding
			Compare(valueModel, RootContext, ParentContext, ChildContext);
		}

		[Test]
		public void TestContextAddedAfterBinding()
		{
			var valueModel = BindTestModel();

			RootContext.AddContext(ParentContext);
			ParentContext.AddContext(ChildContext);
			//parent and child contexts should contain this same value binding
			Compare(valueModel, RootContext, ParentContext, ChildContext);
		}

		[Test]
		public void TestSiblingsShareBinding()
		{
			RootContext.AddContext(ParentContext);
			ParentContext.AddContext(ChildContext);
			var ChildContextTwo = new HierarchicalContext(view, ContextStartupFlags.AUTOMATIC | ContextStartupFlags.MANUAL_HIERARCHY);
			ParentContext.AddContext(ChildContextTwo);

			//Set up the root context first
			var valueModel = BindTestModel();

			//both children should be the same
			Compare(valueModel, ChildContext, ChildContextTwo);
		}

		[Test]
		public void TestRemoveContextClearsBindings()
		{
			RootContext.AddContext(ParentContext);
			ParentContext.AddContext(ChildContext);

			//Set up the root context first
			var valueModel = BindTestModel();

			ParentContext.RemoveContext(ChildContext);
			Assert.Throws<InjectionException>(
				() => ChildContext.injectionBinder.GetInstance<TestModel>()
			);

		}

		[Test]
		public void TestRemoveAndAddToNewParent()
		{
			//If I remove from a parent and move to a new parent, 
			//I should have the new parent bindings

			RootContext.AddContext(ParentContext);
			RootContext.AddContext(ChildContext); //Directly to root this time


		}

		[Test]
		public void TestMovingDownHierarchy()
		{
			//Assuming a Root -> Child and Root -> Parent relationship
			//If I move Child from Root to Parent, I should inherit Parent overries
			//This might remove existing bindings and readd them, which is fine
			//It should retain non overriden

		}

		[Test]
		public void TestMovingUpHierarchy()
		{
			//Basically the same thing, but in reverse
		}

		[Test]
		public void TestInheritedBindingsAreOverriddenLocally()
		{
			//inherited bindings can be overriden, as they are weak
		}

		[Test]
		public void TestOverrideNotRemovedByParent()
		{
			//If we override with a local binding
			//Make sure we do not remove it when cleaning up parent inherited bindings
			
		}

		[Test]
		public void TestOverrideRemovesChildBindings()
		{
			//On a local override
			//Remove all child bindings
		}

		[Test]
		public void TestLocalOverrideInheritedByChildren()
		{
			//If a local override is also an inherited binding
			//Remove the previous inherited binding and bind the new
			//cascade to all children
		}

		[Test]
		public void TestImplicitInheritedBindings()
		{

		}


		private TestModel BindTestModel()
		{
			TestModel valueModel = new TestModel();
			RootContext.injectionBinder.Bind<TestModel>().ToValue(valueModel).Inherited(); //bind it once here and it should be accessible everywhere
			return valueModel;
		}

		private void Compare(TestModel expected, params HierarchicalContext[] contexts)
		{
			foreach (var context in contexts)
			{
				Assert.AreSame(expected, context.injectionBinder.GetInstance<TestModel>());
			}
		}

	}
}
