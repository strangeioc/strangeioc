using System;
using System.Diagnostics;
using NUnit.Framework;
using strange.framework.api;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestBinding
	{
		IBinding binding;

		[SetUp]
		public void Setup()
		{
			binding = new Binding ();
		}

		[TearDown]
		public void TearDown()
		{
			binding = null;
		}

		[Test]
		public void TestKeyAsType ()
		{
			binding.Key<MarkerClass> ();
			Assert.That (binding.key == typeof(MarkerClass));
		}

		[Test]
		public void TestKeyAsIntType ()
		{
			binding.Key<int> ();
			Type typeOfInt = typeof(int);
			Assert.That (binding.key == typeOfInt);
		}

		[Test]
		public void TestKeyAsObject ()
		{
			MarkerClass value = new MarkerClass ();
			binding.Key (value);

			Assert.That (binding.key == value);
		}

		[Test]
		public void TestKeyToAsTypes()
		{
			binding.Key<InjectableSuperClass> ().To<InjectableDerivedClass> ();
			Assert.That (binding.key == typeof(InjectableSuperClass));
			object[] values = binding.value as object[];
			Assert.That (values[0] == typeof(InjectableDerivedClass));
		}

		[Test]
		public void TestKeyToAsStrings()
		{
			const string TEST_KEY = "Test Key";
			const string TEST_VALUE = "Test result value";
			binding.Key(TEST_KEY).To(TEST_VALUE);
			Assert.That (TEST_KEY == binding.key as string);
			object[] values = binding.value as object[];
			Assert.That (TEST_VALUE == values[0] as string);
		}

		[Test]
		public void TestIntToValue()
		{
			const int TEST_VALUE = 42;
			binding.Key<int>().To(TEST_VALUE);
			Assert.That (typeof(int) == binding.key);
			object[] values = binding.value as object[];
			Assert.That (TEST_VALUE == (int)values[0]);
		}

		[Test]
		public void TestNameAsType()
		{
			binding.Key<InjectableSuperClass> ().To<InjectableDerivedClass> ().ToName<MarkerClass> ();
			Assert.That (binding.name == typeof(MarkerClass));
		}

		[Test]
		public void TestNameToValue()
		{
			binding.Key<InjectableSuperClass> ().To<InjectableDerivedClass> ().ToName (SomeEnum.FOUR);
			Assert.That ((SomeEnum)binding.name == SomeEnum.FOUR);
		}

		[Test]
		public void TestKeyToWithMultipleChainedValues()
		{
			ClassWithConstructorParameters test1 = new ClassWithConstructorParameters (1, "abc");
			ClassWithConstructorParameters test2 = new ClassWithConstructorParameters (2, "def");
			ClassWithConstructorParameters test3 = new ClassWithConstructorParameters (3, "ghi");

			binding.Key<ISimpleInterface> ().To (test1).To (test2).To (test3);
			Assert.That (binding.key == typeof(ISimpleInterface));

			object[] values = binding.value as object[];
			Assert.IsNotNull (values);
			Assert.That (values.Length == 3);
			for(int a = 0; a < values.Length; a++)
			{
				ISimpleInterface value = values [a] as ISimpleInterface;
				Assert.IsNotNull (value);
				Assert.That (value.intValue == a + 1);
			}
		}

		[Test]
		public void TestOneToOneConstrainedBinding()
		{
			ClassWithConstructorParameters test1 = new ClassWithConstructorParameters (1, "abc");
			ClassWithConstructorParameters test2 = new ClassWithConstructorParameters (2, "def");
			ClassWithConstructorParameters test3 = new ClassWithConstructorParameters (3, "ghi");

			binding.valueConstraint = BindingConstraintType.ONE;
			binding.Key<ISimpleInterface> ().To (test1).To (test2).To (test3);
			Assert.That (binding.key == typeof(ISimpleInterface));
			Assert.That (binding.value is ClassWithConstructorParameters);
			Assert.That ((binding.value as ClassWithConstructorParameters).intValue == 3);

			//Clean up
			binding.valueConstraint = BindingConstraintType.MANY;
		}
	}
}

