using System;
using NUnit.Framework;
using strange.framework.api;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestSemiBinding
	{
		ISemiBinding semibinding;

		[SetUp]
		public void Setup()
		{
			semibinding = new SemiBinding ();
		}

		[TearDown]
		public void TearDown()
		{
			semibinding = null;
		}

		[Test]
		public void TestType ()
		{
			semibinding.Add(typeof(TestSemiBinding));
			Assert.AreEqual (typeof(TestSemiBinding), semibinding.value);
		}

		[Test]
		public void TestIntType ()
		{
			semibinding.Add(typeof(int));
			Type typeOfInt = typeof(int);
			Assert.AreEqual (typeOfInt, semibinding.value);
		}

		[Test]
		public void TestObject ()
		{
			ClassWithConstructorParameters o = new ClassWithConstructorParameters (42, "abc");
			semibinding.Add (o);
			Assert.AreEqual (o, semibinding.value);
			Assert.AreEqual (42, o.intValue);
		}

		[Test]
		public void TestOverwriteSingleSemibinding ()
		{
			ClassWithConstructorParameters o = new ClassWithConstructorParameters (42, "abc");
			semibinding.Add (o);
			ClassWithConstructorParameters o1 = new ClassWithConstructorParameters (43, "def");
			semibinding.Add (o1);
			ClassWithConstructorParameters o2 = new ClassWithConstructorParameters (44, "ghi");
			semibinding.Add (o2);
			Assert.AreNotEqual (o, semibinding.value);
			Assert.AreEqual (o2, semibinding.value);
			Assert.AreEqual (44, o2.intValue);
		}

		[Test]
		public void TestRemoveFromSingleSemibinding ()
		{
			semibinding.constraint = BindingConstraintType.ONE;

			ClassWithConstructorParameters o = new ClassWithConstructorParameters (42, "abc");
			semibinding.Add (o);

			ClassWithConstructorParameters value = semibinding.value as ClassWithConstructorParameters;

			Assert.AreEqual (o, value);
			Assert.AreEqual (42, value.intValue);

			semibinding.Remove (o);

			Assert.IsNull (semibinding.value);
		}

		[Test]
		public void TestMultiSemibinding ()
		{
			semibinding.constraint = BindingConstraintType.MANY;

			ClassWithConstructorParameters o = new ClassWithConstructorParameters (42, "abc");
			semibinding.Add (o);
			ClassWithConstructorParameters o1 = new ClassWithConstructorParameters (43, "def");
			semibinding.Add (o1);
			ClassWithConstructorParameters o2 = new ClassWithConstructorParameters (44, "ghi");
			semibinding.Add (o2);

			object[] values = semibinding.value as object[];
			Assert.AreEqual (3, values.Length);
			ClassWithConstructorParameters value = values [2] as ClassWithConstructorParameters;
			Assert.AreEqual (o2, value);
			Assert.AreEqual (44, value.intValue);
		}

		[Test]
		public void TestRemoveFromMultiSemibinding ()
		{
			semibinding.constraint = BindingConstraintType.MANY;

			ClassWithConstructorParameters o = new ClassWithConstructorParameters (42, "abc");
			semibinding.Add (o);
			ClassWithConstructorParameters o1 = new ClassWithConstructorParameters (43, "def");
			semibinding.Add (o1);
			ClassWithConstructorParameters o2 = new ClassWithConstructorParameters (44, "ghi");
			semibinding.Add (o2);

			object[] before = semibinding.value as object[];
			Assert.AreEqual (3, before.Length);
			ClassWithConstructorParameters beforeValue = before [2] as ClassWithConstructorParameters;
			Assert.AreEqual (o2, beforeValue);
			Assert.AreEqual (44, beforeValue.intValue);

			semibinding.Remove (o1);

			object[] after = semibinding.value as object[];
			Assert.AreEqual (2, after.Length);
			ClassWithConstructorParameters afterValue = after [1] as ClassWithConstructorParameters;
			Assert.AreEqual (o2, afterValue);
			Assert.AreEqual (44, afterValue.intValue);
		}
	}
}

