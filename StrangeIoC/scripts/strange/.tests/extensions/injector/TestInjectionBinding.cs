using System;
using System.Diagnostics;
using NUnit.Framework;
using strange.framework.api;
using strange.framework.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;


namespace strange.unittests
{
	[TestFixture()]
	public class TestInjectionBinding
	{
		[Test]
		public void TestDefaultType ()
		{
			const string TEST_KEY = "TEST_KEY";
			Binder.BindingResolver resolver = delegate (IBinding binding)
			{
				(binding as IInjectionBinding).type = InjectionBindingType.DEFAULT;
				Assert.That (TEST_KEY == binding.value as string);
				Assert.That ((binding as InjectionBinding).type == InjectionBindingType.DEFAULT);
			};
			InjectionBinding defaultBinding = new InjectionBinding (resolver);
			defaultBinding.To (TEST_KEY);
		}

		[Test]
		public void TestSingletonType ()
		{
			const string TEST_KEY = "TEST_KEY";
			Binder.BindingResolver resolver = delegate (IBinding binding)
			{
				(binding as IInjectionBinding).type = InjectionBindingType.SINGLETON;
				Assert.That (TEST_KEY == binding.value as string);
				Assert.That ((binding as InjectionBinding).type == InjectionBindingType.SINGLETON);
			};
			InjectionBinding defaultBinding = new InjectionBinding (resolver);
			defaultBinding.To (TEST_KEY);
		}

		[Test]
		public void TestValueType ()
		{
			const string TEST_KEY = "TEST_KEY";
			Binder.BindingResolver resolver = delegate (IBinding binding)
			{
				(binding as IInjectionBinding).type = InjectionBindingType.VALUE;
				Assert.That (TEST_KEY == binding.value as string);
				Assert.That ((binding as InjectionBinding).type == InjectionBindingType.VALUE);
			};
			InjectionBinding defaultBinding = new InjectionBinding (resolver);
			defaultBinding.To (TEST_KEY);
		}

		[Test]
		public void TestSingletonChainBinding ()
		{
			int a = 0;

			Binder.BindingResolver resolver = delegate (IBinding binding)
			{
				Assert.That (binding.value == typeof(InjectableDerivedClass));
				InjectionBindingType correctType = (a == 0) ? InjectionBindingType.DEFAULT : InjectionBindingType.SINGLETON;
				Assert.That ((binding as InjectionBinding).type == correctType);
				a++;
			};
			new InjectionBinding (resolver).Bind<InjectableSuperClass> ().To<InjectableDerivedClass> ().ToSingleton ();
		}

		[Test]
		public void TestValueChainBinding ()
		{
			int a = 0;
			InjectableDerivedClass testValue = new InjectableDerivedClass ();

			Binder.BindingResolver resolver = delegate (IBinding binding)
			{
				if (a == 2)
				{
					Assert.That (binding.value == testValue);
					InjectionBindingType correctType = (a == 0) ? InjectionBindingType.DEFAULT : InjectionBindingType.VALUE;
					Assert.That ((binding as InjectionBinding).type == correctType);
				}
				a++;
			};
			new InjectionBinding (resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToValue (testValue);
		}

		[Test]
		public void TestIllegalValueBinding ()
		{
			MarkerClass illegalValue = new MarkerClass ();

			Binder.BindingResolver resolver = delegate (IBinding binding){};
			TestDelegate testDelegate = delegate()
			{
				new InjectionBinding (resolver).Bind<InjectableSuperClass> ().To<InjectableDerivedClass> ().ToValue (illegalValue);
			};
			InjectionException ex = 
				Assert.Throws<InjectionException> (testDelegate);
			Assert.That (ex.type == InjectionExceptionType.ILLEGAL_BINDING_VALUE);
		}

		[Test]
		public void TestSupplyOne()
		{
			Binder.BindingResolver resolver = delegate (IBinding bound)
			{
				object[] value = (bound as IInjectionBinding).GetSupply();
				Assert.AreEqual(value[0], typeof(HasANamedInjection));
			};
			InjectionBinding binding = new InjectionBinding (resolver);
			binding.SupplyTo<HasANamedInjection> ();
		}

		[Test]
		public void TestSupplySeveral()
		{
			Type[] supplied = new Type[3];
			supplied [0] = typeof (HasANamedInjection);
			supplied [1] = typeof (HasANamedInjection2);
			supplied [2] = typeof (InjectsClassToBeInjected);
			int iterator = 0;
			int resolveIterator = 0;

			Binder.BindingResolver resolver = delegate (IBinding bound)
			{
				object[] value = (bound as IInjectionBinding).GetSupply();
				Assert.AreEqual(value[value.Length-1], supplied[iterator]);
				resolveIterator ++;
			};

			InjectionBinding binding = new InjectionBinding (resolver);

			while (iterator < 3)
			{
				binding.SupplyTo (supplied[iterator]);
				iterator++;
			}

			Assert.AreEqual (3, resolveIterator);
		}

		[Test]
		public void TestUnsupply()
		{
			Binder.BindingResolver resolver = delegate (IBinding bound)
			{
			};
			InjectionBinding binding = new InjectionBinding (resolver);
			binding.To<ClassToBeInjected>().SupplyTo<HasANamedInjection> ();
			Assert.AreEqual (typeof(HasANamedInjection), binding.GetSupply()[0]);
			Assert.AreEqual (typeof(ClassToBeInjected), binding.value);

			binding.Unsupply<HasANamedInjection> ();
			Assert.IsNull (binding.GetSupply());
		}

		[Test]
		public void TestGetSupply()
		{
			Type[] supplied = new Type[3];
			supplied [0] = typeof (HasANamedInjection);
			supplied [1] = typeof (HasANamedInjection2);
			supplied [2] = typeof (InjectsClassToBeInjected);
			int iterator = 0;

			Binder.BindingResolver resolver = delegate (IBinding bound)
			{
				object[] value = (bound as IInjectionBinding).GetSupply();
				Assert.AreEqual(value[value.Length-1], supplied[iterator]);
			};

			InjectionBinding binding = new InjectionBinding (resolver);

			while (iterator < 3)
			{
				binding.SupplyTo (supplied[iterator]);
				iterator++;
			}

			object[] supply = binding.GetSupply ();
			Assert.AreEqual (3, supply.Length);

			for (var a = 0; a < supply.Length; a++)
			{
				Assert.AreEqual (supply[a], supplied[a]);
			}
		}
	}
}

