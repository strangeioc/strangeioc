using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture]
	public class TestInjector
	{
		IInjectionBinder binder;

		[SetUp]
		public void SetUp()
		{
			binder = new InjectionBinder ();
		}

		[Test]
		public void TestTaggedConstructor ()
		{
			binder.Bind<ClassWithConstructorParameters> ().To<ClassWithConstructorParameters> ();
			binder.Bind<int> ().ToValue (42);
			binder.Bind<string> ().ToValue ("Liberator");
			ClassWithConstructorParameters instance = 
				binder.GetInstance<ClassWithConstructorParameters> () as ClassWithConstructorParameters;
			Assert.IsNotNull (instance);
			Assert.AreEqual (42, instance.intValue);
			Assert.AreEqual ("Liberator", instance.stringValue);
		}

		[Test]
		public void TestForcedParameterConstruction ()
		{
			binder.Bind<ClassWithConstructorParametersOnlyOneConstructor> ().To<ClassWithConstructorParametersOnlyOneConstructor> ();
			binder.Bind<string> ().ToValue ("Zaphod");
			ClassWithConstructorParametersOnlyOneConstructor instance = 
				binder.GetInstance<ClassWithConstructorParametersOnlyOneConstructor> () as ClassWithConstructorParametersOnlyOneConstructor;
			Assert.IsNotNull (instance);
			Assert.That (instance.stringVal == "Zaphod");
		}

		[Test]
		public void TestPostConstruct ()
		{
			binder.Bind<PostConstructClass> ().To<PostConstructClass> ();
			binder.Bind<float> ().ToValue ((float)Math.PI);
			PostConstructClass instance = binder.GetInstance<PostConstructClass> () as PostConstructClass;
			Assert.IsNotNull (instance);
			Assert.That (instance.floatVal == (float)Math.PI * 2f);
		}

		//RE: Issue #23. A value-mapped object must never attempt to re-instantiate
		[Test]
		public void TestValueMappingWithConstructorArguments()
		{
			string stringVal = "Ender Wiggin";
			ClassWithConstructorParametersOnlyOneConstructor instance = new ClassWithConstructorParametersOnlyOneConstructor (stringVal);
			binder.Bind<ClassWithConstructorParametersOnlyOneConstructor> ().ToValue (instance);
			//If this class attempts to construct, with no string mapped, there'll be an error
			ClassWithConstructorParametersOnlyOneConstructor instance2 = binder.GetInstance<ClassWithConstructorParametersOnlyOneConstructor> () as ClassWithConstructorParametersOnlyOneConstructor;
			Assert.AreSame (instance, instance2);
			Assert.AreEqual (stringVal, instance2.stringVal);
		}

		[Test]
		public void TestMultiplePostConstructs ()
		{
			binder.Bind<PostConstructTwo> ().To<PostConstructTwo> ();
			binder.Bind<float> ().ToValue ((float)Math.PI);
			PostConstructTwo instance = binder.GetInstance<PostConstructTwo> () as PostConstructTwo;
			Assert.IsNotNull (instance);
			Assert.That (instance.floatVal == (float)Math.PI * 4f);
		}

		[Test]
		public void TestNamedInjections ()
		{
			InjectableSuperClass testValue = new InjectableSuperClass ();
			float defaultFloatValue = testValue.floatValue;
			testValue.floatValue = 3.14f;

			binder.Bind<int> ().ToValue (20);
			binder.Bind<InjectableSuperClass> ().To<InjectableSuperClass> ().ToName (SomeEnum.ONE);
			binder.Bind<InjectableSuperClass> ().ToName<MarkerClass> ().ToValue(testValue);
			binder.Bind<HasNamedInjections> ().To<HasNamedInjections> ();
			HasNamedInjections instance = binder.GetInstance<HasNamedInjections> () as HasNamedInjections;
			Assert.IsNotNull (instance);
			Assert.IsNotNull (instance.injectionOne);
			Assert.IsNotNull (instance.injectionTwo);
			Assert.AreEqual (20, instance.injectionOne.intValue);
			Assert.AreEqual (20, instance.injectionTwo.intValue);
			Assert.That (instance.injectionOne.floatValue == defaultFloatValue);
			Assert.That (instance.injectionTwo.floatValue == 3.14f);
		}

		[Test]
		public void TestCircularDependencies()
		{
			binder.Bind<CircularDependencyOne> ().To<CircularDependencyOne> ();
			binder.Bind<CircularDependencyTwo> ().To<CircularDependencyTwo> ();

			TestDelegate testDelegate = delegate()
			{
				binder.GetInstance<CircularDependencyOne>();
			};
			InjectionException ex = Assert.Throws<InjectionException>(testDelegate);
			Assert.That (ex.type == InjectionExceptionType.CIRCULAR_DEPENDENCY);
		}
	}
}

