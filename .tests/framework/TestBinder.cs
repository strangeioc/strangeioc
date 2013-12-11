using System;
using NUnit.Framework;
using strange.framework.api;
using strange.framework.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestBinder
	{
		IBinder binder;

		[SetUp]
		public void SetUp()
		{
			binder = new Binder ();
		}

		[Test]
		public void TestBindType ()
		{
			binder.Bind<InjectableSuperClass> ().To<InjectableDerivedClass> ();
			IBinding binding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNotNull (binding);
			Assert.That (binding.key == typeof(InjectableSuperClass));
			Assert.That (binding.key != typeof(InjectableDerivedClass));
			Assert.That (binding.value != typeof(InjectableSuperClass));
			Assert.That ((binding.value as object[])[0] == typeof(InjectableDerivedClass));
		}

		[Test]
		public void TestBindValue ()
		{
			object testKeyValue = new MarkerClass ();
			binder.Bind(testKeyValue).To<InjectableDerivedClass> ();
			IBinding binding = binder.GetBinding (testKeyValue);
			Assert.IsNotNull (binding);
			Assert.That (binding.key == testKeyValue);
			Assert.That ((binding.value as object[])[0] == typeof(InjectableDerivedClass));
		}

		[Test]
		public void TestNamedBinding ()
		{
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ();
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ().ToName<MarkerClass>();
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ().ToName("strange");

			//Test the unnamed binding
			IBinding unnamedBinding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNotNull (unnamedBinding);
			Assert.That (unnamedBinding.key == typeof(InjectableSuperClass));
			Type unnamedBindingValue = (unnamedBinding.value as object[]) [0] as Type;
			Assert.That (unnamedBindingValue == typeof(InjectableDerivedClass));
			Assert.That (unnamedBinding.name != typeof(MarkerClass));

			//Test the binding named by type
			IBinding namedBinding = binder.GetBinding<InjectableSuperClass> (typeof(MarkerClass));
			Assert.IsNotNull (namedBinding);
			Assert.That (namedBinding.key == typeof(InjectableSuperClass));
			Type namedBindingValue = (namedBinding.value as object[]) [0] as Type;
			Assert.That (namedBindingValue == typeof(InjectableDerivedClass));
			Assert.That (namedBinding.name == typeof(MarkerClass));

			//Test the binding named by string
			IBinding namedBinding2 = binder.GetBinding<InjectableSuperClass> ("strange");
			Assert.IsNotNull (namedBinding2);
			Assert.That (namedBinding2.key == typeof(InjectableSuperClass));
			Type namedBinding2Value = (namedBinding2.value as object[]) [0] as Type;
			Assert.That (namedBinding2Value == typeof(InjectableDerivedClass));
			Assert.That ((string)namedBinding2.name == "strange");
		}

		[Test]
		public void TestRemoveBindingByKey ()
		{
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ();
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ().ToName<MarkerClass>();

			//Test the unnamed binding
			IBinding unnamedBinding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNotNull (unnamedBinding);
			Assert.That (unnamedBinding.key == typeof(InjectableSuperClass));
			Type unnamedBindingValue = (unnamedBinding.value as object[]) [0] as Type;
			Assert.That (unnamedBindingValue == typeof(InjectableDerivedClass));
			Assert.That (unnamedBinding.name != typeof(MarkerClass));

			//Test the binding named by type
			IBinding namedBinding = binder.GetBinding<InjectableSuperClass> (typeof(MarkerClass));
			Assert.IsNotNull (namedBinding);
			Assert.That (namedBinding.key == typeof(InjectableSuperClass));
			Type namedBindingValue = (namedBinding.value as object[]) [0] as Type;
			Assert.That (namedBindingValue == typeof(InjectableDerivedClass));
			Assert.That (namedBinding.name == typeof(MarkerClass));

			//Remove the first binding
			binder.Unbind<InjectableSuperClass> ();
			IBinding removedUnnamedBinding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNull (removedUnnamedBinding);

			//Ensure the named binding still exists
			IBinding unremovedNamedBinding = binder.GetBinding<InjectableSuperClass> (typeof(MarkerClass));
			Assert.IsNotNull (unremovedNamedBinding);
		}

		[Test]
		public void TestRemoveBindingByKeyAndName ()
		{
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ();
			IBinding namedBinding = binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ().ToName<MarkerClass>();

			//Remove the first binding
			binder.Unbind (namedBinding.key, namedBinding.name);
			IBinding removedNamedBinding = binder.GetBinding<InjectableSuperClass> (typeof(MarkerClass));
			Assert.IsNull (removedNamedBinding);

			//Ensure the unnamed binding still exists
			IBinding unremovedUnnamedBinding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNotNull (unremovedUnnamedBinding);
		}

		[Test]
		public void TestRemoveBindingByBinding ()
		{
			IBinding unnamedBinding = binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ();
			binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass> ().ToName<MarkerClass>();

			//Remove the first binding
			binder.Unbind (unnamedBinding);
			IBinding removedUnnamedBinding = binder.GetBinding<InjectableSuperClass> ();
			Assert.IsNull (removedUnnamedBinding);

			//Ensure the named binding still exists
			IBinding unremovedNamedBinding = binder.GetBinding<InjectableSuperClass> (typeof(MarkerClass));
			Assert.IsNotNull (unremovedNamedBinding);
		}

		[Test]
		public void TestRemoveValueFromBinding()
		{
			IBinding binding = binder.Bind<float> ().To (1).To (2).To (3);
			object[] before = binding.value as object[];
			Assert.AreEqual (3, before.Length);

			binder.RemoveValue (binding, 2);

			object[] after = binding.value as object[];
			Assert.AreEqual (2, after.Length);
			Assert.AreEqual (1, after[0]);
			Assert.AreEqual (3, after[1]);
		}

		[Test]
		public void TestNullValueInBinding()
		{
			IBinding binding = binder.Bind<float> ().To (1).To (2).To (3);
			object[] before = binding.value as object[];
			Assert.AreEqual (3, before.Length);

			binder.RemoveValue (binding, before);

			object[] after = binding.value as object[];
			Assert.IsNull (after);
		}

		[Test]
		public void TestConflictWithoutWeak()
		{
			binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>();

			TestDelegate testDelegate = delegate
			{
				binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementerTwo>();
				object instance = binder.GetBinding<ISimpleInterface>().value;
				Assert.IsNotNull(instance);
			};

			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we have a conflict between the two above bindings
			Assert.AreEqual (BinderExceptionType.CONFLICT_IN_BINDER, ex.type);
		}
		[Test]
		public void TestWeakBindings()
		{
			SimpleInterfaceImplementer one = new SimpleInterfaceImplementer();
			SimpleInterfaceImplementerTwo two = new SimpleInterfaceImplementerTwo();
			IBinding binding = binder.Bind<ISimpleInterface>().To(one).Weak();

			binding.valueConstraint = BindingConstraintType.ONE;
			TestDelegate testDelegate = delegate
			{
				binder.Bind<ISimpleInterface>().To(two).valueConstraint = BindingConstraintType.ONE;
				IBinding retrievedBinding = binder.GetBinding<ISimpleInterface>();
				Assert.NotNull(retrievedBinding);
				Assert.NotNull(retrievedBinding.value);
				Console.WriteLine(retrievedBinding.value);
				Assert.AreEqual(retrievedBinding.value, two);
			};

			Assert.DoesNotThrow(testDelegate); //Second binding "two" overrides weak binding "one"

		}

	}
}
