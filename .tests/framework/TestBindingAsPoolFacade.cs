using System;
using NUnit.Framework;
using strange.framework.api;
using strange.framework.impl;
using System.Collections;

namespace strange.unittests
{
	[TestFixture()]
	public class TestBindingAsPoolFacade
	{
		Binding b;
		IPool binding;


		[SetUp]
		public void Setup()
		{
			b = new Binding ();
			b.ToPool ();

			binding = (b as IPool);
		}

		[TearDown]
		public void TearDown()
		{
			b = null;
		}

		[Test]
		public void TestSetPoolProperties()
		{
			Assert.AreEqual (0, binding.Size);

			binding.Size = 100;
			Assert.AreEqual (100, binding.Size);

			binding.OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
			Assert.AreEqual (PoolOverflowBehavior.EXCEPTION, binding.OverflowBehavior);

			binding.OverflowBehavior = PoolOverflowBehavior.WARNING;
			Assert.AreEqual (PoolOverflowBehavior.WARNING, binding.OverflowBehavior);

			binding.OverflowBehavior = PoolOverflowBehavior.IGNORE;
			Assert.AreEqual (PoolOverflowBehavior.IGNORE, binding.OverflowBehavior);

			binding.InflationType = PoolInflationType.DOUBLE;
			Assert.AreEqual (PoolInflationType.DOUBLE, binding.InflationType);

			binding.InflationType = PoolInflationType.INCREMENT;
			Assert.AreEqual (PoolInflationType.INCREMENT, binding.InflationType);
		}

		[Test]
		public void TestGetInstance()
		{
			binding.Size = 4;
			for (int a = 0; a < binding.Size; a++)
			{
				b.To (new ClassToBeInjected ());
			}

			for (int a = binding.Size; a > 0; a--)
			{
				Assert.AreEqual (a, binding.Available);
				ClassToBeInjected instance = binding.GetInstance () as ClassToBeInjected;
				Assert.IsNotNull (instance);
				Assert.IsInstanceOf<ClassToBeInjected> (instance);
				Assert.AreEqual (a - 1, binding.Available);
			}
		}

		[Test]
		public void TestReturnInstance()
		{
			binding.Size = 4;
			Stack stack = new Stack (binding.Size);
			for (int a = 0; a < binding.Size; a++)
			{
				b.To (new ClassToBeInjected ());
			}

			for (int a = 0; a < binding.Size; a++)
			{
				stack.Push(binding.GetInstance ());
			}

			Assert.AreEqual (binding.Size, stack.Count);
			Assert.AreEqual (0, binding.Available);

			for (int a = 0; a < binding.Size; a++)
			{
				binding.ReturnInstance (stack.Pop ());
			}

			Assert.AreEqual (0, stack.Count);
			Assert.AreEqual (binding.Size, binding.Available);
		}

		[Test]
		public void TestClean()
		{
			binding.Size = 4;
			for (int a = 0; a < binding.Size; a++)
			{
				b.To (new ClassToBeInjected ());
			}
			binding.Clean ();
			Assert.AreEqual (0, binding.Available);
		}

		[Test]
		public void TestPoolOverflowException()
		{
			binding.Size = 4;
			for (int a = 0; a < binding.Size; a++)
			{
				b.To (new ClassToBeInjected ());
			}

			for (int a = binding.Size; a > 0; a--)
			{
				Assert.AreEqual (a, binding.Available);
				binding.GetInstance ();
			}

			TestDelegate testDelegate = delegate()
			{
				binding.GetInstance();
			};
			PoolException ex = Assert.Throws<PoolException> (testDelegate);
			Assert.That (ex.type == PoolExceptionType.OVERFLOW);
		}

		[Test]
		public void TestOverflowWithoutException()
		{
			binding.Size = 4;
			binding.OverflowBehavior = PoolOverflowBehavior.IGNORE;
			for (int a = 0; a < binding.Size; a++)
			{
				b.To (new ClassToBeInjected ());
			}

			for (int a = binding.Size; a > 0; a--)
			{
				Assert.AreEqual (a, binding.Available);
				binding.GetInstance ();
			}

			TestDelegate testDelegate = delegate()
			{
				object shouldBeNull = binding.GetInstance();
				Assert.IsNull(shouldBeNull);
			};
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestPoolTypeMismatchException()
		{
			binding.Size = 4;
			b.To (new ClassToBeInjected ());

			TestDelegate testDelegate = delegate()
			{
				b.To(new InjectableDerivedClass());
			};
			PoolException ex = Assert.Throws<PoolException> (testDelegate);
			Assert.That (ex.type == PoolExceptionType.TYPE_MISMATCH);
		}

		[Test]
		public void TestRemoveFromPool()
		{
			binding.Size = 4;
			for (int a = 0; a < binding.Size; a++)
			{
				b.To (new ClassToBeInjected ());
			}

			for (int a = binding.Size; a > 0; a--)
			{
				Assert.AreEqual (a, binding.Available);
				ClassToBeInjected instance = binding.GetInstance () as ClassToBeInjected;
				b.RemoveValue (instance);
			}

			Assert.AreEqual (0, binding.Available);
		}

		[Test]
		public void TestRemovalException()
		{
			binding.Size = 4;
			b.To (new ClassToBeInjected ());
			TestDelegate testDelegate = delegate()
			{
				b.RemoveValue (new InjectableDerivedClass ());
			};
			PoolException ex = Assert.Throws<PoolException> (testDelegate);
			Assert.That (ex.type == PoolExceptionType.TYPE_MISMATCH);
		}

		[Test]
		public void TestReleaseOfPoolable()
		{
			binding.Size = 4;
			b.To (new PooledInstanceForBinding ());
			PooledInstanceForBinding instance = binding.GetInstance () as PooledInstanceForBinding;
			instance.someValue = 42;
			Assert.AreEqual (42, instance.someValue);
			binding.ReturnInstance (instance);
			Assert.AreEqual (0, instance.someValue);
		}

		[Test]
		public void TestExceptionsIfNotPool()
		{
			Binding failBinding = new Binding ();
			IPool fb = (failBinding as IPool);

			assertThrowsFailedFacade (
				delegate(){Console.WriteLine(fb.Available);}
			);
			assertThrowsFailedFacade (
				delegate(){fb.Clean();}
			);
			assertThrowsFailedFacade (
				delegate(){fb.GetInstance();}
			);
			assertThrowsFailedFacade (
				delegate(){Console.WriteLine(fb.InflationType);}
			);
			assertThrowsFailedFacade (
				delegate(){fb.InflationType = PoolInflationType.DOUBLE;}
			);
			assertThrowsFailedFacade (
				delegate(){Console.WriteLine(fb.OverflowBehavior);}
			);
			assertThrowsFailedFacade (
				delegate(){fb.OverflowBehavior = PoolOverflowBehavior.WARNING;}
			);
			assertThrowsFailedFacade (
				delegate(){fb.ReturnInstance("hello");}
			);
			assertThrowsFailedFacade (
				delegate(){Console.WriteLine(fb.Size);}
			);
			assertThrowsFailedFacade (
				delegate(){fb.Size = 1;}
			);
		}

		private void assertThrowsFailedFacade(TestDelegate testDelegate)
		{
			PoolException ex1 = Assert.Throws<PoolException>(testDelegate);
			Assert.That (ex1.type == PoolExceptionType.FAILED_FACADE);
		}
	}

	class PooledInstanceForBinding : IPoolable
	{
		public int someValue = 0;

		public void Release ()
		{
			someValue = 0;
		}
	}
}

