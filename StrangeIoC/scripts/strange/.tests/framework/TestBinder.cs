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
		
		[Test]
		public void TestSimpleRuntimeBinding()
		{
			string jsonString = "[{\"Bind\":\"This\",\"To\":\"That\"},{\"Bind\":[\"Han\"],\"To\":\"Solo\"},{\"Bind\":\"Jedi\",\"To\":[\"Luke\",\"Yoda\",\"Ben\"]}]";
			binder.ConsumeBindings(jsonString);

			IBinding binding = binder.GetBinding ("This");
			Assert.NotNull (binding);
			Assert.AreEqual (binding.key, "This");
			Assert.AreEqual ((binding.value as object[])[0], "That");

			IBinding binding2 = binder.GetBinding ("Han");
			Assert.NotNull (binding2);
			Assert.AreEqual (binding2.key, "Han");
			Assert.AreEqual ((binding2.value as object[])[0], "Solo");

			IBinding binding3 = binder.GetBinding ("Jedi");
			Assert.NotNull (binding3);
			Assert.AreEqual (binding3.key, "Jedi");
			Assert.AreEqual ((binding3.value as object[])[0], "Luke");
			Assert.AreEqual ((binding3.value as object[])[1], "Yoda");
			Assert.AreEqual ((binding3.value as object[])[2], "Ben");

		}

		[Test]
		public void TestRuntimeWeakBinding()
		{
			string jsonString = "[{\"Bind\":\"This\",\"To\":\"That\", \"Options\":\"Weak\"}, {\"Bind\":[\"Han\"],\"To\":\"Solo\"}]";
			binder.ConsumeBindings(jsonString);

			IBinding binding = binder.GetBinding ("This");
			Assert.NotNull (binding);
			Assert.IsTrue (binding.isWeak);

			IBinding binding2 = binder.GetBinding ("Han");
			Assert.NotNull (binding2);
			Assert.IsFalse (binding2.isWeak);
		}

		[Test]
		public void TestRuntimeNoBindException()
		{
			string jsonString = "[{\"oops\":\"Han\",\"To\":\"Solo\"}]";
			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we have no bind key
			Assert.AreEqual (BinderExceptionType.RUNTIME_NO_BIND, ex.type);
		}

		[Test]
		public void TestRuntimeTooManyKeysException()
		{
			string jsonString = "[{\"Bind\":[\"Han\",\"Leia\"],\"To\":\"Solo\"}]";
			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we have two keys in a Binder that only supports one
			Assert.AreEqual (BinderExceptionType.RUNTIME_TOO_MANY_KEYS, ex.type);
		}

		[Test]
		public void TestRuntimeUnknownTypeException()
		{
			string jsonString = "[{\"Bind\":true,\"To\":\"Solo\"}]";
			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because we can't bind a boolean as a key
			Assert.AreEqual (BinderExceptionType.RUNTIME_TYPE_UNKNOWN, ex.type);
		}

		[Test]
		public void TestRuntimeFailedWhitelistException()
		{
			string jsonString = "[{\"Bind\":\"Han\",\"To\":\"Solo\"}, {\"Bind\":\"Darth\",\"To\":\"Vader\"}]";

			System.Collections.Generic.List<object> whitelist = new System.Collections.Generic.List<object> ();
			whitelist.Add ("Solo");
			binder.WhitelistBindings (whitelist);

			TestDelegate testDelegate = delegate
			{
				binder.ConsumeBindings(jsonString);
			};
			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because Vader is not whitelisted
			Assert.AreEqual (BinderExceptionType.RUNTIME_FAILED_WHITELIST_CHECK, ex.type);
		}

	}
}
