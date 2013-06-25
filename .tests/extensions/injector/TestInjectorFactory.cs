using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestInjectorFactory
	{
		IInjectorFactory factory;
		Binder.BindingResolver resolver;

		[SetUp]
		public void SetUp()
		{
			factory = new InjectorFactory ();
			resolver = delegate (IBinding binding){};
		}

		[Test]
		public void TestInstantiation ()
		{
			IInjectionBinding defaultBinding = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ();
			InjectableDerivedClass testResult = factory.Get (defaultBinding) as InjectableDerivedClass;
			Assert.IsNotNull (testResult);
		}

		[Test]
		public void TestInstantiationFactory ()
		{
			IInjectionBinding defaultBinding = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ();
			InjectableDerivedClass testResult = factory.Get (defaultBinding) as InjectableDerivedClass;
			Assert.IsNotNull (testResult);
			int defaultValue = testResult.intValue;
			//Set a value
			testResult.intValue = 42;
			//Now get an instance again and ensure it's a different instance
			InjectableDerivedClass testResult2 = factory.Get (defaultBinding) as InjectableDerivedClass;
			Assert.That (testResult2.intValue == defaultValue);
		}

		[Test]
		public void TestInstantiateSingleton ()
		{
			IInjectionBinding defaultBinding = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ().ToSingleton();
			InjectableDerivedClass testResult = factory.Get (defaultBinding) as InjectableDerivedClass;
			Assert.IsNotNull (testResult);
			//Set a value
			testResult.intValue = 42;
			//Now get an instance again and ensure it's the same instance
			InjectableDerivedClass testResult2 = factory.Get (defaultBinding) as InjectableDerivedClass;
			Assert.That (testResult2.intValue == 42);
		}
		
		[Test]
		public void TestNamedInstances ()
		{
			//Create two named instances
			IInjectionBinding defaultBinding = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ().ToName (SomeEnum.ONE);
			IInjectionBinding defaultBinding2 = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ().ToName (SomeEnum.TWO);

			InjectableDerivedClass testResult = factory.Get (defaultBinding) as InjectableDerivedClass;
			int defaultValue = testResult.intValue;
			Assert.IsNotNull (testResult);
			//Set a value
			testResult.intValue = 42;

			//Now get an instance again and ensure it's a different instance
			InjectableDerivedClass testResult2 = factory.Get (defaultBinding2) as InjectableDerivedClass;
			Assert.IsNotNull (testResult2);
			Assert.That (testResult2.intValue == defaultValue);
		}

		//NOTE: Technically this test is redundant with the test above, since a named instance
		//is a de-facto Singleton
		[Test]
		public void TestNamedSingletons ()
		{
			//Create two named singletons
			IInjectionBinding defaultBinding = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ().ToName (SomeEnum.ONE).ToSingleton();
			IInjectionBinding defaultBinding2 = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ().ToName (SomeEnum.TWO).ToSingleton();

			InjectableDerivedClass testResult = factory.Get (defaultBinding) as InjectableDerivedClass;
			int defaultValue = testResult.intValue;
			Assert.IsNotNull (testResult);
			//Set a value
			testResult.intValue = 42;

			//Now get an instance again and ensure it's a different instance
			InjectableDerivedClass testResult2 = factory.Get (defaultBinding2) as InjectableDerivedClass;
			Assert.IsNotNull (testResult2);
			Assert.That (testResult2.intValue == defaultValue);
		}

		[Test]
		public void TestValueMap()
		{
			InjectableDerivedClass testvalue = new InjectableDerivedClass ();
			testvalue.intValue = 42;
			IInjectionBinding binding = new InjectionBinding (resolver).Key<InjectableSuperClass> ().To <InjectableDerivedClass> ().ToValue (testvalue);
			InjectableDerivedClass testResult = factory.Get (binding) as InjectableDerivedClass;
			Assert.IsNotNull (testResult);
			Assert.That (testResult.intValue == testvalue.intValue);
			Assert.That (testResult.intValue == 42);
		}

		[Test]
		public void TestImplicitBinding()
		{
			//This succeeds if no error
			IInjectionBinding binding = new InjectionBinding(resolver).Bind<InjectableSuperClass> ();
			factory.Get (binding);

			//Succeeds if throws error
			IInjectionBinding binding2 = new InjectionBinding(resolver).Bind<ISimpleInterface> ();
			TestDelegate testDelegate = delegate()
			{
				factory.Get (binding2);
			};
			InjectionException ex = Assert.Throws<InjectionException>(testDelegate);
			Assert.That (ex.type == InjectionExceptionType.NOT_INSTANTIABLE);

			//Succeeds if throws error
			IInjectionBinding binding3 = new InjectionBinding(resolver).Bind<AbstractClass> ();
			TestDelegate testDelegate2 = delegate()
			{
				factory.Get(binding3);
			};
			InjectionException ex2 = Assert.Throws<InjectionException>(testDelegate2);
			Assert.That (ex2.type == InjectionExceptionType.NOT_INSTANTIABLE);
		}

		[Test]
		public void TestImplicitToSingleton()
		{
			//This succeeds if no error
			IInjectionBinding binding = new InjectionBinding(resolver).Bind<InjectableSuperClass> ().ToSingleton ();
			factory.Get (binding);

			//Succeeds if throws error
			IInjectionBinding binding2 = new InjectionBinding(resolver).Bind<ISimpleInterface> ().ToSingleton ();
			TestDelegate testDelegate = delegate()
			{
				factory.Get (binding2);
			};
			InjectionException ex = Assert.Throws<InjectionException>(testDelegate);
			Assert.That (ex.type == InjectionExceptionType.NOT_INSTANTIABLE);

			//Succeeds if throws error
			IInjectionBinding binding3 = new InjectionBinding(resolver).Bind<AbstractClass> ().ToSingleton ();
			TestDelegate testDelegate2 = delegate()
			{
				factory.Get(binding3);
			};
			InjectionException ex2 = Assert.Throws<InjectionException>(testDelegate2);
			Assert.That (ex2.type == InjectionExceptionType.NOT_INSTANTIABLE);
		}
	}
}

