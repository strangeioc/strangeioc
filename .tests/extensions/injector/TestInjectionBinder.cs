using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.reflector.api;
using strange.extensions.pool.api;
using strange.extensions.pool.impl;
using strange.framework.api;
using System.Collections.Generic;

namespace strange.unittests
{
	[TestFixture()]
	public class TestinjectionBinder
	{
		IInjectionBinder binder;

		[SetUp]
		public void SetUp()
		{
			binder = new InjectionBinder ();
            
		}

		[TearDown]
		public void TearDown()
		{
			PostConstructSimple.PostConstructCount = 0;
		}

		[Test]
		public void TestInjectorExists()
		{
			Assert.That (binder.injector != null);
		}

		[Test]
		public void TestGetBindingFlat ()
		{
			binder.Bind<InjectableSuperClass> ().To<InjectableSuperClass> ();
			IInjectionBinding binding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNotNull (binding);
		}

		[Test]
		public void TestGetBindingAbstract ()
		{
			binder.Bind<ISimpleInterface> ().To<ClassWithConstructorParameters> ();
			IInjectionBinding binding = binder.GetBinding<ISimpleInterface> ();
			Assert.IsNotNull (binding);
		}

		[Test]
		public void TestGetNamedBinding ()
		{
			binder.Bind<ISimpleInterface> ().To<ClassWithConstructorParameters> ().ToName<MarkerClass>();
			IInjectionBinding binding = binder.GetBinding<ISimpleInterface> (typeof(MarkerClass));
			Assert.IsNotNull (binding);
		}

		[Test]
		public void TestGetInstance1()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();

			ClassToBeInjected instance = binder.GetInstance (typeof(ClassToBeInjected)) as ClassToBeInjected;

			Assert.IsNotNull (instance);
			Assert.That (instance is ClassToBeInjected);
		}

		[Test]
		public void TestGetInstance2()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();

			ClassToBeInjected instance = binder.GetInstance<ClassToBeInjected> ();

			Assert.IsNotNull (instance);
			Assert.That (instance is ClassToBeInjected);
		}

		[Test]
		public void TestGetNamedInstance1()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().ToName<MarkerClass>();

			ClassToBeInjected instance = binder.GetInstance (typeof(ClassToBeInjected), typeof(MarkerClass)) as ClassToBeInjected;

			Assert.IsNotNull (instance);
			Assert.That (instance is ClassToBeInjected);
		}

		[Test]
		public void TestGetNamedInstance2()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().ToName<MarkerClass>();

			ClassToBeInjected instance = binder.GetInstance<ClassToBeInjected> (typeof(MarkerClass));

			Assert.IsNotNull (instance);
			Assert.That (instance is ClassToBeInjected);
		}

		[Test]
		public void TestGetNamedInstance3()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().ToName(SomeEnum.ONE);

			ClassToBeInjected instance = binder.GetInstance (typeof(ClassToBeInjected), SomeEnum.ONE) as ClassToBeInjected;

			Assert.IsNotNull (instance);
			Assert.That (instance is ClassToBeInjected);
		}

		[Test]
		public void TestGetNamedInstance4()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().ToName(SomeEnum.ONE);

			ClassToBeInjected instance = binder.GetInstance<ClassToBeInjected> (SomeEnum.ONE);

			Assert.IsNotNull (instance);
			Assert.That (instance is ClassToBeInjected);
		}

		[Test]
		public void TestInjectionErrorFailureToProvideDependency()
		{
			TestDelegate testDelegate = delegate() {
				binder.GetInstance<InjectableSuperClass> ();
			};
			binder.Bind<InjectableSuperClass> ().To<InjectableSuperClass> ();
			InjectionException ex = Assert.Throws<InjectionException> (testDelegate);
			Assert.That (ex.type == InjectionExceptionType.NULL_BINDING);
		}

		[Test]
		public void TestInjectionProvideIntDependency()
		{
			binder.Bind<InjectableSuperClass> ().To<InjectableSuperClass> ();
			binder.Bind<int> ().ToValue (42);
			InjectableSuperClass testValue = binder.GetInstance<InjectableSuperClass> ();
			Assert.IsNotNull (testValue);
			Assert.That (testValue.intValue == 42);
		}

		[Test]
		public void TestRemoveDependency()
		{
			binder.Bind<InjectableSuperClass> ().To<InjectableSuperClass> ();
			binder.Bind<int> ().ToValue (42);
			InjectableSuperClass testValueBeforeUnbinding = binder.GetInstance<InjectableSuperClass> ();
			Assert.IsNotNull (testValueBeforeUnbinding);
			Assert.That (testValueBeforeUnbinding.intValue == 42);

			binder.Unbind<int> ();

			TestDelegate testDelegate = delegate() {
				binder.GetInstance<InjectableSuperClass> ();
			};

			InjectionException ex = Assert.Throws<InjectionException> (testDelegate);
			Assert.That (ex.type == InjectionExceptionType.NULL_BINDING);
		}

		[Test]
		public void TestValueToSingleton()
		{
			GuaranteedUniqueInstances uniqueInstance = new GuaranteedUniqueInstances ();
			binder.Bind<GuaranteedUniqueInstances> ().ToValue (uniqueInstance);
			GuaranteedUniqueInstances instance1 = binder.GetInstance <GuaranteedUniqueInstances> ();
			GuaranteedUniqueInstances instance2 = binder.GetInstance <GuaranteedUniqueInstances> ();
			Assert.AreEqual (instance1.uid, instance2.uid);
			Assert.AreSame (instance1, instance2);
		}

		//RE: Issue #23. A value-mapping trumps a Singleton mapping
		[Test]
		public void TestValueToSingletonBinding ()
		{
			InjectableSuperClass instance = new InjectableSuperClass ();
			InjectionBinding binding = binder.Bind<InjectableSuperClass> ().ToValue (instance).ToSingleton() as InjectionBinding;
			Assert.AreEqual (InjectionBindingType.VALUE, binding.type);
		}

		//RE: Issue #23. A value-mapping trumps a Singleton mapping
		[Test]
		public void TestSingletonToValueBinding ()
		{
			InjectableSuperClass instance = new InjectableSuperClass ();
			InjectionBinding binding = binder.Bind<InjectableSuperClass> ().ToSingleton().ToValue (instance) as InjectionBinding;
			Assert.AreEqual (InjectionBindingType.VALUE, binding.type);
		}

		//RE:Issue #34. Ensure that a Singleton instance can properly use constructor injection
		[Test]
		public void TestConstructorToSingleton()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();
			binder.Bind<ConstructorInjectsClassToBeInjected>().To<ConstructorInjectsClassToBeInjected>().ToSingleton();
			ConstructorInjectsClassToBeInjected instance = binder.GetInstance<ConstructorInjectsClassToBeInjected>();
			Assert.IsNotNull (instance.injected);
		}

		//RE: Issue #32. A value-bound injection should not post-construct twice
		//The PostConstruct fires when the class is requested.
		[Test]
		public void TestDoublePostConstruct()
		{
            PostConstructSimple.PostConstructCount = 0;
			PostConstructSimple instance = new PostConstructSimple ();
			binder.Bind<PostConstructSimple> ().ToValue (instance);
			binder.Bind<InjectsPostConstructSimple> ().To<InjectsPostConstructSimple>();

			InjectsPostConstructSimple instance1 = binder.GetInstance<InjectsPostConstructSimple> ();
			InjectsPostConstructSimple instance2 = binder.GetInstance<InjectsPostConstructSimple> ();

			Assert.AreSame (instance, instance1.pcs);
			Assert.AreNotSame (instance1, instance2);
			Assert.AreEqual (1, PostConstructSimple.PostConstructCount);
		}

		[Test]
		public void TestPolymorphicBinding()
		{
			binder.Bind<ISimpleInterface> ().Bind<IAnotherSimpleInterface> ().To<PolymorphicClass> ();

			ISimpleInterface callOnce = binder.GetInstance<ISimpleInterface> ();
			Assert.NotNull (callOnce);
			Assert.IsInstanceOf<PolymorphicClass> (callOnce);

			IAnotherSimpleInterface callAgain = binder.GetInstance<IAnotherSimpleInterface> ();
			Assert.NotNull (callAgain);
			Assert.IsInstanceOf<PolymorphicClass> (callAgain);
		}

		[Test]
		public void TestPolymorphicSingleton()
		{
			binder.Bind<ISimpleInterface> ().Bind<IAnotherSimpleInterface> ().To<PolymorphicClass> ().ToSingleton();

			ISimpleInterface callOnce = binder.GetInstance<ISimpleInterface> ();
			Assert.NotNull (callOnce);
			Assert.IsInstanceOf<PolymorphicClass> (callOnce);

			IAnotherSimpleInterface callAgain = binder.GetInstance<IAnotherSimpleInterface> ();
			Assert.NotNull (callAgain);
			Assert.IsInstanceOf<PolymorphicClass> (callAgain);

			callOnce.intValue = 42;

			Assert.AreSame (callOnce, callAgain);
			Assert.AreEqual (42, (callAgain  as ISimpleInterface).intValue);
		}

		[Test]
		public void TestNamedInstanceBeforeUnnamedInstance()
		{
			binder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ().ToName(SomeEnum.ONE);
			binder.Bind<ISimpleInterface> ().To<PolymorphicClass> ();

			ISimpleInterface instance1 = binder.GetInstance<ISimpleInterface> (SomeEnum.ONE);
			ISimpleInterface instance2 = binder.GetInstance<ISimpleInterface> ();

			Assert.That (instance1 is SimpleInterfaceImplementer);
			Assert.That (instance2 is PolymorphicClass);
		}


		[Test]
		public void TestUnnamedInstanceBeforeNamedInstance()
		{
			binder.Bind<ISimpleInterface> ().To<PolymorphicClass> ();
			binder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ().ToName(SomeEnum.ONE);

			ISimpleInterface instance1 = binder.GetInstance<ISimpleInterface> (SomeEnum.ONE);
			ISimpleInterface instance2 = binder.GetInstance<ISimpleInterface> ();

			Assert.That (instance1 is SimpleInterfaceImplementer);
			Assert.That (instance2 is PolymorphicClass);
		}

		[Test]
		public void TestPrereflectOne()
		{
			binder.Bind<ISimpleInterface> ().Bind<IAnotherSimpleInterface> ().To<PolymorphicClass> ();

			System.Collections.Generic.List<Type> list = new System.Collections.Generic.List<Type> ();
			list.Add (typeof(PolymorphicClass));
			int count = binder.Reflect (list);

			Assert.AreEqual (1, count);

			IReflectedClass reflected = binder.injector.reflector.Get<PolymorphicClass> ();
			Assert.True (reflected.preGenerated);
		}

		[Test]
		public void TestPrereflectMany()
		{
			binder.Bind<HasNamedInjections> ().To<HasNamedInjections> ();
			binder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ().ToName(SomeEnum.ONE);
			binder.Bind<ISimpleInterface> ().To<PolymorphicClass> ();
			binder.Bind<InjectableSuperClass> ().To<InjectableDerivedClass> ();
            binder.Bind<int>().ToValue(42);
            binder.Bind<string>().ToValue("zaphod"); //primitives won't get reflected...

			System.Collections.Generic.List<Type> list = new System.Collections.Generic.List<Type> ();
			list.Add (typeof(HasNamedInjections));
			list.Add (typeof(SimpleInterfaceImplementer));
			list.Add (typeof(PolymorphicClass));
			list.Add (typeof(InjectableDerivedClass));
            list.Add(typeof(int));

			int count = binder.Reflect (list);
            Assert.AreEqual(4, count);             //...so list length will not include primitives

			IReflectedClass reflected1 = binder.injector.reflector.Get<HasNamedInjections> ();
			Assert.True (reflected1.preGenerated);

			IReflectedClass reflected2 = binder.injector.reflector.Get<SimpleInterfaceImplementer> ();
			Assert.True (reflected2.preGenerated);

			IReflectedClass reflected3 = binder.injector.reflector.Get<PolymorphicClass> ();
			Assert.True (reflected3.preGenerated);
			Assert.AreNotEqual (reflected2.constructor, reflected3.constructor);

			IReflectedClass reflected4 = binder.injector.reflector.Get<InjectableDerivedClass> ();
			Assert.True (reflected4.preGenerated);
		}

		[Test]
		public void TestPrereflectAll()
		{
			binder.Bind<HasNamedInjections> ().To<HasNamedInjections> ();
			binder.Bind<ISimpleInterface> ().To<SimpleInterfaceImplementer> ().ToName (SomeEnum.ONE);
			binder.Bind<ISimpleInterface> ().To<PolymorphicClass> ();
			binder.Bind<InjectableSuperClass> ().To<InjectableDerivedClass> ();
			binder.Bind<int> ().ToValue (42);
			binder.Bind<string> ().ToValue ("zaphod"); //primitives won't get reflected...

			int count = binder.ReflectAll ();
			Assert.AreEqual (4, count);             //...so list length will not include primitives

			ISimpleInterface s = binder.GetInstance<ISimpleInterface> ();
			Assert.IsTrue (s is PolymorphicClass);

			IReflectedClass reflected1 = binder.injector.reflector.Get<HasNamedInjections> ();
			Assert.True (reflected1.preGenerated);

			IReflectedClass reflected2 = binder.injector.reflector.Get<SimpleInterfaceImplementer> ();
			Assert.True (reflected2.preGenerated);

			IReflectedClass reflected3 = binder.injector.reflector.Get<PolymorphicClass> ();
			Assert.True (reflected3.preGenerated);
			Assert.AreNotEqual (reflected2.constructor, reflected3.constructor);

			IReflectedClass reflected4 = binder.injector.reflector.Get<InjectableDerivedClass> ();
			Assert.True (reflected4.preGenerated);

		}

		[Test]
		public void TestGetPoolInjection()
		{
			//Pool requires an instance provider. This InjectionBinder is that provider for the integrated test.
			//In MVCSContext this is handled automatically.
			binder.Bind<IInstanceProvider> ().ToValue (binder);

			binder.Bind<SimpleInterfaceImplementer> ().To<SimpleInterfaceImplementer> ();
			binder.Bind<Pool<SimpleInterfaceImplementer>> ().ToSingleton();
			binder.Bind<IUsesPool> ().To<UsesPool> ().ToSingleton();

			IUsesPool instance = binder.GetInstance<IUsesPool>();

			Assert.IsNotNull (instance);
			Assert.IsNotNull (instance.Instance1);
			Assert.IsNotNull (instance.Instance2);
		}
	}

	interface ITestPooled : IPoolable
	{
	}

	class TestPooled : ITestPooled
	{
		public void Restore ()
		{
			throw new NotImplementedException ();
		}

		public void Retain()
		{

		}

		public void Release()
		{

		}

		public bool retain{ get; set; }
	}

	interface IUsesPool
	{
		ISimpleInterface Instance1{ get; set; }
		ISimpleInterface Instance2{ get; set; }
	}

	class UsesPool : IUsesPool
	{
		[Inject]
		public Pool<SimpleInterfaceImplementer> pool { get; set; }

		public ISimpleInterface Instance1{ get; set; }
		public ISimpleInterface Instance2{ get; set; }

		[PostConstruct]
		public void PostConstruct()
		{
			Instance1 = pool.GetInstance () as ISimpleInterface;
			Instance2 = pool.GetInstance () as ISimpleInterface;
		}
	}
}

