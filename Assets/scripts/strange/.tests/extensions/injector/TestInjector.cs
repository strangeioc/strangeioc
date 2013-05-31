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
			binder.Bind<int> ().AsValue (42);
			ClassWithConstructorParameters instance = 
				binder.GetInstance<ClassWithConstructorParameters> () as ClassWithConstructorParameters;
			Assert.IsNotNull (instance);
			Assert.That (instance.intValue == 42);
		}

		[Test]
		public void TestForcedParameterConstruction ()
		{
			binder.Bind<ClassWithConstructorParametersOnlyOneConstructor> ().To<ClassWithConstructorParametersOnlyOneConstructor> ();
			binder.Bind<string> ().AsValue ("Zaphod");
			ClassWithConstructorParametersOnlyOneConstructor instance = 
				binder.GetInstance<ClassWithConstructorParametersOnlyOneConstructor> () as ClassWithConstructorParametersOnlyOneConstructor;
			Assert.IsNotNull (instance);
			Assert.That (instance.stringVal == "Zaphod");
		}

		[Test]
		public void TestPostConstruct ()
		{
			binder.Bind<PostConstructClass> ().To<PostConstructClass> ();
			binder.Bind<float> ().AsValue ((float)Math.PI);
			PostConstructClass instance = binder.GetInstance<PostConstructClass> () as PostConstructClass;
			Assert.IsNotNull (instance);
			Assert.That (instance.floatVal == (float)Math.PI * 2f);
		}

		[Test]
		public void TestNamedInjections ()
		{
			InjectableSuperClass testValue = new InjectableSuperClass ();
			float defaultFloatValue = testValue.floatValue;
			testValue.floatValue = 3.14f;

			binder.Bind<int> ().AsValue (0);
			binder.Bind<InjectableSuperClass> ().To<InjectableSuperClass> ().ToName (SomeEnum.ONE);
			binder.Bind<InjectableSuperClass> ().ToName<MarkerClass> ().AsValue(testValue);
			binder.Bind<HasNamedInjections> ().To<HasNamedInjections> ();
			HasNamedInjections instance = binder.GetInstance<HasNamedInjections> () as HasNamedInjections;
			Assert.IsNotNull (instance);
			Assert.IsNotNull (instance.injectionOne);
			Assert.IsNotNull (instance.injectionTwo);
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

