using NUnit.Framework;
using strange.extensions.reflector.api;
using strange.extensions.reflector.impl;
using System.Reflection;

namespace strange.unittests
{
	[TestFixture()]
	public class TestReflectionBinder
	{
		IReflectionBinder reflector;

		[SetUp]
		public void SetUp()
		{
			reflector = new ReflectionBinder ();
		}

		[Test]
		public void TestFoundSoleConstructor ()
		{

		}

		[Test]
		public void TestConstructorWithNoParameters ()
		{
			IReflectedClass reflected = reflector.Get<ClassToBeInjected> ();
			Assert.AreEqual (0, reflected.ConstructorParameters.Length);
		}

		[Test]
		public void TestConstructorWithSingleParameter ()
		{
			IReflectedClass reflected = reflector.Get<ClassWithConstructorParametersOnlyOneConstructor> ();
			Assert.AreEqual (1, reflected.ConstructorParameters.Length);
		}

		[Test]
		public void TestTaggedConstructor ()
		{
			IReflectedClass reflected = reflector.Get<ClassWithConstructorParameters> ();
			Assert.AreEqual (2, reflected.ConstructorParameters.Length);

			ConstructorInfo constructor = reflected.Constructor;
			object[] parameters = new object[2];
			parameters [0] = 42;
			parameters [1] = "Zaphod";
			ClassWithConstructorParameters instance = constructor.Invoke(parameters) as ClassWithConstructorParameters;
			Assert.IsNotNull (instance);
			Assert.AreEqual (42, instance.intValue);
			Assert.AreEqual ("Zaphod", instance.stringValue);
		}

		[Test]
		public void TestShortestConstructor ()
		{
			IReflectedClass reflected = reflector.Get<MultipleConstructorsUntagged> ();
			Assert.AreEqual (3, reflected.ConstructorParameters.Length);

			ISimpleInterface simple = new SimpleInterfaceImplementer ();
			simple.intValue = 11001001;

			ConstructorInfo constructor = reflected.Constructor;
			object[] parameters = new object[3];
			parameters [0] = simple;
			parameters [1] = 42;
			parameters [2] = "Zaphod";
			MultipleConstructorsUntagged instance = constructor.Invoke(parameters) as MultipleConstructorsUntagged;
			Assert.IsNotNull (instance);
			Assert.AreEqual (simple.intValue, instance.simple.intValue);
			Assert.AreEqual (42, instance.intValue);
			Assert.AreEqual ("Zaphod", instance.stringValue);
		}

		[Test]
		public void TestConstructorWithMultipleParameters ()
		{
			IReflectedClass reflected = reflector.Get<ClassWithConstructorParameters> ();
			Assert.AreEqual (2, reflected.ConstructorParameters.Length);
		}

		[Test]
		public void TestConstructorNamedInjection() 
		{
			IReflectedClass reflected = reflector.Get<ConstructorNamedInjection>();

			Assert.That(reflected.ConstructorParameters.Length == 2);
			Assert.That(reflected.ConstructorParameterNames.Length == 2);
		}

		[Test]
		public void TestSinglePostConstruct ()
		{
			IReflectedClass reflected = reflector.Get<PostConstructClass> ();
			Assert.AreEqual (1, reflected.PostConstructors.Length);
		}

		[Test]
		public void TestMultiplePostConstructs ()
		{
			IReflectedClass reflected = reflector.Get<PostConstructTwo> ();
			Assert.AreEqual (2, reflected.PostConstructors.Length);
		}

		[Test]
		public void TestSingleSetter ()
		{
			IReflectedClass reflected = reflector.Get<PostConstructTwo> ();
			Assert.AreEqual (1, reflected.Setters.Length);
			Assert.IsNull (reflected.Setters[0].name);

			ReflectedAttribute attr = reflected.Setters [0];
			Assert.AreEqual (attr.type, typeof(float));
		}

		[Test]
		public void TestInheritedInjectionHiding()
		{
			IReflectedClass overrideBase = reflector.Get<BaseInheritanceOverride>();
			IReflectedClass overrideExtended = reflector.Get<ExtendedInheritanceOverride>();

			Assert.AreEqual(1, overrideBase.Setters.Length);
			Assert.AreEqual(1, overrideExtended.Setters.Length);
		}

		[Test]
		public void TestVirtualOverrideInjection()
		{
			IReflectedClass overrideBase = reflector.Get<BaseInheritanceOverride>();
			IReflectedClass overrideExtended = reflector.Get<ExtendedInheritanceImplied>();

			Assert.AreEqual(1, overrideBase.Setters.Length);
			Assert.AreEqual(1, overrideExtended.Setters.Length);
		}

		[Test]
		public void TestInjectedTypeForOverrideIsCorrect()
		{
			IReflectedClass overrideBase = reflector.Get<BaseInheritanceOverride>();
			IReflectedClass overrideExtended = reflector.Get<ExtendedInheritanceOverride>();

			Assert.AreEqual(overrideExtended.Setters[0].type, typeof(IExtendedInterface));
		}

		[Test]
		public void TestMultipleLevelsOfInheritance()
		{
			IReflectedClass overrideBase = reflector.Get<BaseInheritanceOverride>();
			IReflectedClass overrideExtended = reflector.Get<ExtendedInheritanceOverride>();
			IReflectedClass overrideExtendedTwo = reflector.Get<ExtendedInheritanceOverrideTwo>();

			Assert.AreEqual(1, overrideBase.Setters.Length);
			Assert.AreEqual(1, overrideExtended.Setters.Length);
			Assert.AreEqual(1, overrideExtendedTwo.Setters.Length);
		}

		[Test]
		public void TestMultipleSetters ()
		{
			IReflectedClass reflected = reflector.Get<HasTwoInjections> ();
			Assert.AreEqual (2, reflected.Setters.Length);
			Assert.IsNull (reflected.Setters[0].name);

			bool foundStringType = false;
			bool foundInjectableSuperClassType = false;

			foreach (ReflectedAttribute attr in reflected.Setters)
			{
				if (attr.type == typeof(string))
				{
					foundStringType = true;
					Assert.AreEqual ("injectionTwo", attr.propertyInfo.Name);
				}
				if (attr.type == typeof(InjectableSuperClass))
				{
					foundInjectableSuperClassType = true;
					Assert.AreEqual ("injectionOne", attr.propertyInfo.Name);
				}
			}
			Assert.True (foundStringType);
			Assert.True (foundInjectableSuperClassType);
		}

		[Test]
		public void TestNamedSetters ()
		{
			IReflectedClass reflected = reflector.Get<HasNamedInjections> ();
			Assert.AreEqual (2, reflected.Setters.Length);

			int a = 0;
			int injectableSuperClassCount = 0;
			bool foundSomeEnum = false;
			bool foundMarkerClass = false;

			foreach (ReflectedAttribute attr in reflected.Setters)
			{
				if (attr.type == typeof(InjectableSuperClass))
				{
					injectableSuperClassCount ++;

					if (attr.name.Equals(SomeEnum.ONE))
					{
						Assert.False (foundSomeEnum);
						foundSomeEnum = true;
					}
					if (attr.name.Equals(typeof(MarkerClass)))
					{
						Assert.False (foundMarkerClass);
						foundMarkerClass = true;
					}
				}
				a++;
			}

			Assert.AreEqual (2, injectableSuperClassCount);
		}

		[Test]
		public void TestSettersOnDerivedClass()
		{
			IReflectedClass reflected = reflector.Get<InjectableDerivedClass> ();
			Assert.AreEqual (2, reflected.Setters.Length);

			bool foundIntType = false;
			bool foundClassToBeInjectedType = false;

			foreach (ReflectedAttribute attr in reflected.Setters)
			{
				if (attr.type == typeof(int))
				{
					foundIntType = true;
					Assert.AreEqual ("intValue", attr.propertyInfo.Name);
				}
				if (attr.type == typeof(ClassToBeInjected))
				{
					foundClassToBeInjectedType = true;
					Assert.AreEqual ("injected", attr.propertyInfo.Name);
				}
			}
			Assert.True (foundIntType);
			Assert.True (foundClassToBeInjectedType);
		}

		[Test]
		public void TestReflectAnInterface ()
		{
			TestDelegate testDelegate = delegate()
			{
				reflector.Get<ISimpleInterface> ();
			};
			ReflectionException ex = Assert.Throws<ReflectionException>(testDelegate);
			Assert.That (ex.type == ReflectionExceptionType.CANNOT_REFLECT_INTERFACE);
		}

		[Test]
		public void TestReflectionCaching()
		{
			IReflectedClass reflected = reflector.Get<HasNamedInjections> ();
			Assert.False (reflected.PreGenerated);
			IReflectedClass reflected2 = reflector.Get<HasNamedInjections> ();
			Assert.True (reflected2.PreGenerated);
		}

		[Test]
		public void TestNonpublicInjectionMapping()
		{
			TestDelegate testDelegate = delegate()
			{
				reflector.Get<NonpublicInjection> ();
			};
			ReflectionException ex = Assert.Throws<ReflectionException>(testDelegate);
			Assert.That (ex.type == ReflectionExceptionType.CANNOT_INJECT_INTO_NONPUBLIC_SETTER);
		}
	}
}

