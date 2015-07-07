using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.reflector.api;
using strange.extensions.pool.api;
using strange.extensions.pool.impl;
using strange.framework.api;
using System.Collections.Generic;
using strange.framework.impl;

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

		[Test]
		public void TestOverrideInjections()
		{
			binder.Bind<ExtendedInheritanceOverride>().ToSingleton();

			ISimpleInterface simple = new SimpleInterfaceImplementer();
			IExtendedInterface extended = new ExtendedInterfaceImplementer();
			binder.Bind<ISimpleInterface>().ToValue(simple);
			binder.Bind<IExtendedInterface>().ToValue(extended);

			ExtendedInheritanceOverride overridingInjectable = binder.GetInstance<ExtendedInheritanceOverride>();
			Assert.NotNull(overridingInjectable);
			Assert.NotNull(overridingInjectable.MyInterface);
			Assert.AreEqual(overridingInjectable.MyInterface, extended);
		}

		/// <summary>
		/// Test that you will properly inherit values that you aren't overriding
		/// </summary>
		[Test]
		public void TestInheritedInjectionHidingDoesntHappenWithoutHiding()
		{
			binder.Bind<ExtendedInheritanceOverride>().ToSingleton();
			binder.Bind<ExtendedInheritanceNoHide>().ToSingleton();

			ISimpleInterface simple = new SimpleInterfaceImplementer();
			IExtendedInterface extended = new ExtendedInterfaceImplementer();

			binder.Bind<ISimpleInterface>().ToValue(simple);
			binder.Bind<IExtendedInterface>().ToValue(extended);

			ExtendedInheritanceOverride overridingInjectable = binder.GetInstance<ExtendedInheritanceOverride>();
			Assert.NotNull(overridingInjectable);
			Assert.NotNull(overridingInjectable.MyInterface);
			Assert.AreEqual(overridingInjectable.MyInterface, extended);

			ExtendedInheritanceNoHide noHide = binder.GetInstance<ExtendedInheritanceNoHide>();
			Assert.NotNull(noHide);
			Assert.NotNull(noHide.MyInterface);
			Assert.AreEqual(noHide.MyInterface, simple);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ISimpleInterface",
//				"To": "strange.unittests.SimpleInterfaceImplementer"
//			}
//		]
		[Test]
		public void TestSimpleRuntimeInjection()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\"}]";

			binder.ConsumeBindings (jsonString);

			IBinding binding = binder.GetBinding<ISimpleInterface> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IInjectionBinding).type, InjectionBindingType.DEFAULT);

			ISimpleInterface instance = binder.GetInstance (typeof(ISimpleInterface)) as ISimpleInterface;
			Assert.IsInstanceOf<SimpleInterfaceImplementer> (instance);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ISimpleInterface",
//				"To": "strange.unittests.SimpleInterfaceImplementer",
//				"ToName": "Test1",
//				"Options": "ToSingleton"
//			},
//			{
//				"Bind": "strange.unittests.ISimpleInterface",
//				"To": "strange.unittests.SimpleInterfaceImplementer",
//				"ToName": "Test2",
//				"Options": "ToSingleton"
//			}
//		]
		[Test]
		public void TestNamedRuntimeInjection()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\",\"ToName\":\"Test1\",\"Options\":\"ToSingleton\"}, {\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\",\"ToName\":\"Test2\",\"Options\":\"ToSingleton\"}]";

			binder.ConsumeBindings (jsonString);

			IBinding binding = binder.GetBinding<ISimpleInterface> ("Test1");
			Assert.NotNull (binding);

			ISimpleInterface instance = binder.GetInstance (typeof(ISimpleInterface), "Test1") as ISimpleInterface;
			Assert.IsInstanceOf<SimpleInterfaceImplementer> (instance);

			IBinding binding2 = binder.GetBinding<ISimpleInterface> ("Test2");
			Assert.NotNull (binding2);

			ISimpleInterface instance2 = binder.GetInstance (typeof(ISimpleInterface), "Test2") as ISimpleInterface;
			Assert.IsInstanceOf<SimpleInterfaceImplementer> (instance2);

			Assert.AreNotSame (instance, instance2);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.SimpleInterfaceImplementer"
//			}
//		]
		[Test]
		public void TestRuntimeInjectionBindToSelf()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.SimpleInterfaceImplementer\"}]";

			binder.ConsumeBindings (jsonString);

			IBinding binding = binder.GetBinding<SimpleInterfaceImplementer> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IInjectionBinding).type, InjectionBindingType.DEFAULT);

			SimpleInterfaceImplementer instance = binder.GetInstance (typeof(SimpleInterfaceImplementer)) as SimpleInterfaceImplementer;
			Assert.IsInstanceOf<SimpleInterfaceImplementer> (instance);

			ISimpleInterface instance2 = binder.GetInstance (typeof(SimpleInterfaceImplementer)) as ISimpleInterface;
			Assert.AreNotSame (instance, instance2);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ISimpleInterface",
//				"To": "strange.unittests.SimpleInterfaceImplementer",
//				"Options": "ToSingleton"
//			}
//		]
		[Test]
		public void TestRuntimeInjectionSingleton()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\", \"Options\":\"ToSingleton\"}]";

			binder.ConsumeBindings (jsonString);

			IBinding binding = binder.GetBinding<ISimpleInterface> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IInjectionBinding).type, InjectionBindingType.SINGLETON);

			ISimpleInterface instance = binder.GetInstance (typeof(ISimpleInterface)) as ISimpleInterface;
			Assert.IsInstanceOf<SimpleInterfaceImplementer> (instance);

			ISimpleInterface instance2 = binder.GetInstance (typeof(ISimpleInterface)) as ISimpleInterface;
			Assert.AreSame (instance, instance2);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ISimpleInterface",
//				"To": "strange.unittests.SimpleInterfaceImplementer",
//				"Options": [
//					"ToSingleton",
//					"Weak",
//					"CrossContext"
//				]
//			}
//		]
		[Test]
		public void TestRuntimeInjectionCrossContext()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\", \"Options\":[\"ToSingleton\",\"Weak\",\"CrossContext\"]}]";

			binder.ConsumeBindings (jsonString);

			IBinding binding = binder.GetBinding<ISimpleInterface> ();
			Assert.NotNull (binding);
			Assert.IsTrue ((binding as IInjectionBinding).isCrossContext);
			Assert.IsTrue (binding.isWeak);
			Assert.AreEqual ((binding as IInjectionBinding).type, InjectionBindingType.SINGLETON);

			ISimpleInterface instance = binder.GetInstance (typeof(ISimpleInterface)) as ISimpleInterface;
			Assert.IsInstanceOf<SimpleInterfaceImplementer> (instance);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": [
//					"strange.unittests.ISimpleInterface",
//					"strange.unittests.IAnotherSimpleInterface"
//				],
//				"To": "strange.unittests.PolymorphicClass"
//			}
//		]
		[Test]
		public void TestRuntimePolymorphism()
		{
			string jsonString = "[{\"Bind\":[\"strange.unittests.ISimpleInterface\",\"strange.unittests.IAnotherSimpleInterface\"],\"To\":\"strange.unittests.PolymorphicClass\"}]";

			binder.ConsumeBindings (jsonString);

			IBinding binding = binder.GetBinding<ISimpleInterface> ();
			Assert.NotNull (binding);

			ISimpleInterface instance = binder.GetInstance (typeof(ISimpleInterface)) as ISimpleInterface;
			Assert.IsInstanceOf<PolymorphicClass> (instance);

			IBinding binding2 = binder.GetBinding<IAnotherSimpleInterface> ();
			Assert.NotNull (binding2);

			IAnotherSimpleInterface instance2 = binder.GetInstance (typeof(IAnotherSimpleInterface)) as IAnotherSimpleInterface;
			Assert.IsInstanceOf<PolymorphicClass> (instance2);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ISimpleInterface",
//				"To": [
//					"strange.unittests.SimpleInterfaceImplementer",
//					"strange.unittests.PolymorphicClass"
//				]
//			}
//		]
		[Test]
		public void TestRuntimeExceptionTooManyValues()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":[\"strange.unittests.SimpleInterfaceImplementer\",\"strange.unittests.PolymorphicClass\"]}]";

			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we have two values in a Binder that only supports one
			Assert.AreEqual (BinderExceptionType.RUNTIME_TOO_MANY_VALUES, ex.type);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "ISimpleInterface",
//				"To": "strange.unittests.SimpleInterfaceImplementer"
//			}
//		]
		[Test]
		public void TestRuntimeExceptionUnqualifiedKeyException()
		{
			string jsonString = "[{\"Bind\":\"ISimpleInterface\",\"To\":\"strange.unittests.SimpleInterfaceImplementer\"}]";
			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we haven't fully qualified the key
			Assert.AreEqual (BinderExceptionType.RUNTIME_NULL_VALUE, ex.type);
		}

		[Test]
		public void TestRuntimeExceptionUnqualifiedValueException()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ISimpleInterface\",\"To\":\"SimpleInterfaceImplementer\"}]";
			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we haven't fully qualified the value
			Assert.AreEqual (BinderExceptionType.RUNTIME_NULL_VALUE, ex.type);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ClassToBeInjected",
//				"To": "strange.unittests.ClassToBeInjected"
//			},
//			{
//				"Bind": "strange.unittests.ClassToBeInjected",
//				"To": "strange.unittests.ExtendsClassToBeInjected",
//				"ToName": 1,
//				"Options": [
//					{
//						"SupplyTo": "strange.unittests.ConstructorInjectsClassToBeInjected"
//					}
//				]
//			},
//			{
//				"Bind": "strange.unittests.ClassToBeInjected",
//				"To": "strange.unittests.ClassToBeInjected"
//			},
//			{
//				"Bind": "strange.unittests.ConstructorInjectsClassToBeInjected",
//				"To": "strange.unittests.ConstructorInjectsClassToBeInjected"
//			}
//		]
		[Test]
		public void TestRuntimeSimpleSupplyBinding()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ClassToBeInjected\",\"To\":\"strange.unittests.ClassToBeInjected\"},";
			jsonString += "{\"Bind\":\"strange.unittests.ClassToBeInjected\",\"To\":\"strange.unittests.ExtendsClassToBeInjected\",\"ToName\": 1,\"Options\": [{\"SupplyTo\": \"strange.unittests.ConstructorInjectsClassToBeInjected\"}]},";
			jsonString += "{\"Bind\":\"strange.unittests.InjectsClassToBeInjected\",\"To\": \"strange.unittests.InjectsClassToBeInjected\"},";
			jsonString += "{\"Bind\":\"strange.unittests.ConstructorInjectsClassToBeInjected\",\"To\": \"strange.unittests.ConstructorInjectsClassToBeInjected\"}]";

			binder.ConsumeBindings (jsonString);

			InjectsClassToBeInjected instance1 = binder.GetInstance<InjectsClassToBeInjected> ();
			ConstructorInjectsClassToBeInjected instance2 = binder.GetInstance<ConstructorInjectsClassToBeInjected> ();

			Assert.IsInstanceOf<ClassToBeInjected> (instance1.injected);
			Assert.IsInstanceOf<ClassToBeInjected> (instance2.injected);

			Assert.IsNotInstanceOf<ExtendsClassToBeInjected> (instance1.injected);
			Assert.IsInstanceOf<ExtendsClassToBeInjected> (instance2.injected);
		}

//		Supplied JSON
//		[
//			{
//				"Bind": "strange.unittests.ClassToBeInjected",
//				"To": "strange.unittests.ExtendsClassToBeInjected",
//				"Options": [
//					"ToSingleton",
//					{
//						"SupplyTo": [
//							"strange.unittests.HasANamedInjection",
//							"strange.unittests.ConstructorInjectsClassToBeInjected",
//							"strange.unittests.InjectsClassToBeInjected"
//						]
//					}
//				]
//			},
//			{
//				"Bind": "strange.unittests.HasANamedInjection",
//				"To": "strange.unittests.HasANamedInjection"
//			},
//			{
//				"Bind": "strange.unittests.ConstructorInjectsClassToBeInjected",
//				"To": "strange.unittests.ConstructorInjectsClassToBeInjected"
//			},
//			{
//				"Bind": "strange.unittests.InjectsClassToBeInjected",
//				"To": "strange.unittests.InjectsClassToBeInjected"
//			}
//		]
		[Test]
		public void TestRuntimeSupplyBindingWithArray()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.ClassToBeInjected\",\"To\":\"strange.unittests.ExtendsClassToBeInjected\",";
			jsonString += "\"Options\": [\"ToSingleton\",{\"SupplyTo\": [\"strange.unittests.HasANamedInjection\",\"strange.unittests.ConstructorInjectsClassToBeInjected\",\"strange.unittests.InjectsClassToBeInjected\"]}]},";
			jsonString += "{\"Bind\":\"strange.unittests.HasANamedInjection\",\"To\":\"strange.unittests.HasANamedInjection\"},";
			jsonString += "{\"Bind\":\"strange.unittests.ConstructorInjectsClassToBeInjected\",\"To\":\"strange.unittests.ConstructorInjectsClassToBeInjected\"},";
			jsonString += "{\"Bind\":\"strange.unittests.InjectsClassToBeInjected\",\"To\":\"strange.unittests.InjectsClassToBeInjected\"}]";

			binder.ConsumeBindings (jsonString);

			HasANamedInjection instance = binder.GetInstance<HasANamedInjection> ();
			ConstructorInjectsClassToBeInjected instance2 = binder.GetInstance<ConstructorInjectsClassToBeInjected> ();
			InjectsClassToBeInjected instance3 = binder.GetInstance<InjectsClassToBeInjected> ();

			Assert.IsInstanceOf<ClassToBeInjected> (instance.injected);
			Assert.IsInstanceOf<ClassToBeInjected> (instance2.injected);
			Assert.IsInstanceOf<ClassToBeInjected> (instance3.injected);
		}

		public void TestSimpleSupplyBinding()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();
			binder.Bind<ClassToBeInjected> ().To<ExtendsClassToBeInjected> ().ToName(1)
				.SupplyTo<ConstructorInjectsClassToBeInjected>();

			binder.Bind<InjectsClassToBeInjected> ().To<InjectsClassToBeInjected> ();
			binder.Bind<ConstructorInjectsClassToBeInjected> ().To<ConstructorInjectsClassToBeInjected> ();

			InjectsClassToBeInjected instance1 = binder.GetInstance<InjectsClassToBeInjected> ();
			ConstructorInjectsClassToBeInjected instance2 = binder.GetInstance<ConstructorInjectsClassToBeInjected> ();

			Assert.IsInstanceOf<ClassToBeInjected> (instance1.injected);
			Assert.IsInstanceOf<ClassToBeInjected> (instance2.injected);

			Assert.IsNotInstanceOf<ExtendsClassToBeInjected> (instance1.injected);
			Assert.IsInstanceOf<ExtendsClassToBeInjected> (instance2.injected);
		}

		[Test]
		public void TestSupplyOverridesName()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().SupplyTo<HasANamedInjection> ();
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().ToName("CapnJack").SupplyTo<HasANamedInjection2> ();
			binder.Bind<HasANamedInjection>().To<HasANamedInjection> ();
			binder.Bind<HasANamedInjection2>().To<HasANamedInjection2> ();

			HasANamedInjection instance = binder.GetInstance<HasANamedInjection> ();
			Assert.IsInstanceOf<ClassToBeInjected> (instance.injected);

			HasANamedInjection2 instance2 = binder.GetInstance<HasANamedInjection2> ();
			Assert.IsInstanceOf<ClassToBeInjected> (instance2.injected);
		}

		[Test]
		public void TestChainedSupplyBinding()
		{
			binder.Bind<ClassToBeInjected>().To<ExtendsClassToBeInjected>()
				.SupplyTo<HasANamedInjection> ()
				.SupplyTo<ConstructorInjectsClassToBeInjected>()
				.SupplyTo<InjectsClassToBeInjected>();

			binder.Bind<HasANamedInjection>().To<HasANamedInjection> ();
			binder.Bind<ConstructorInjectsClassToBeInjected>().To<ConstructorInjectsClassToBeInjected> ();
			binder.Bind<InjectsClassToBeInjected>().To<InjectsClassToBeInjected> ();

			HasANamedInjection instance = binder.GetInstance<HasANamedInjection> ();
			ConstructorInjectsClassToBeInjected instance2 = binder.GetInstance<ConstructorInjectsClassToBeInjected> ();
			InjectsClassToBeInjected instance3 = binder.GetInstance<InjectsClassToBeInjected> ();

			Assert.IsInstanceOf<ClassToBeInjected> (instance.injected);
			Assert.IsInstanceOf<ClassToBeInjected> (instance2.injected);
			Assert.IsInstanceOf<ClassToBeInjected> (instance3.injected);
		}

		[Test]
		public void TestUnsupplyBinding()
		{
			binder.Bind<ClassToBeInjected> ().To<ExtendsClassToBeInjected> ()
				.ToName("Supplier")
				.SupplyTo<InjectsClassToBeInjected> ();

			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();
			
			binder.Bind<InjectsClassToBeInjected>().To<InjectsClassToBeInjected> ();

			InjectsClassToBeInjected instance = binder.GetInstance<InjectsClassToBeInjected> ();
			Assert.IsInstanceOf<ClassToBeInjected> (instance.injected);
			Assert.IsInstanceOf<ExtendsClassToBeInjected> (instance.injected);

			binder.Unsupply<ClassToBeInjected, InjectsClassToBeInjected> ();

			InjectsClassToBeInjected instance2 = binder.GetInstance<InjectsClassToBeInjected> ();
			Assert.IsInstanceOf<ClassToBeInjected> (instance2.injected);
			Assert.IsNotInstanceOf<ExtendsClassToBeInjected> (instance2.injected);
		}

		[Test]
		public void TestGetSupplier()
		{
			binder.Bind<ClassToBeInjected> ().To<ExtendsClassToBeInjected> ()
				.ToName("Supplier")
				.SupplyTo<InjectsClassToBeInjected> ();

			IInjectionBinding binding = binder.GetSupplier (typeof(ClassToBeInjected), typeof(InjectsClassToBeInjected));

			Assert.IsNotNull (binding);
			Assert.AreEqual (typeof (ClassToBeInjected), (binding.key as object[]) [0]);
			Assert.AreEqual (typeof (ExtendsClassToBeInjected), binding.value);
			Assert.AreEqual (typeof (InjectsClassToBeInjected), binding.GetSupply () [0]);
		}

		[Test]
		public void TestComplexSupplyBinding()
		{
			binder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ().ToName(SomeEnum.ONE);
			binder.Bind<ClassToBeInjected>().To<ExtendsClassToBeInjected>()
				.SupplyTo<HasANamedInjection> ()
				.SupplyTo<ConstructorInjectsClassToBeInjected>();
			binder.Bind<HasANamedInjection>().To<HasANamedInjection> ();
			binder.Bind<HasANamedInjection2>().To<HasANamedInjection2> ();
			binder.Bind<ConstructorInjectsClassToBeInjected>().To<ConstructorInjectsClassToBeInjected> ();

			HasANamedInjection instance = binder.GetInstance<HasANamedInjection> ();
			ConstructorInjectsClassToBeInjected instance2 = binder.GetInstance<ConstructorInjectsClassToBeInjected> ();
			HasANamedInjection2 instance3 = binder.GetInstance<HasANamedInjection2> ();

			Assert.IsInstanceOf<ExtendsClassToBeInjected> (instance.injected);
			Assert.IsInstanceOf<ExtendsClassToBeInjected> (instance2.injected);
			Assert.IsNotInstanceOf<ExtendsClassToBeInjected> (instance3.injected);

			binder.Unsupply<ClassToBeInjected, HasANamedInjection> ();

			HasANamedInjection instance4 = binder.GetInstance<HasANamedInjection> ();
			Assert.IsNotInstanceOf<ExtendsClassToBeInjected> (instance4.injected);
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

	class ExtendedInterfaceImplementer : IExtendedInterface
	{
		public int intValue { get; set; }
		public bool Extended()
		{
			return true;
		}
	}
}

