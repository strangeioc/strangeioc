using System;
using NUnit.Framework;
using strange.extensions.pool.api;
using strange.extensions.pool.impl;
using System.Collections;
using strange.framework.api;

namespace strange.unittests
{
	[TestFixture()]
	public class TestPool
	{
		Pool<ClassToBeInjected> pool;


		[SetUp]
		public void Setup()
		{
			pool = new Pool<ClassToBeInjected> ();
		}

		[TearDown]
		public void TearDown()
		{
			pool = null;
		}

		[Test]
		public void TestSetPoolProperties()
		{
			Assert.AreEqual (0, pool.Size);

			pool.Size = 100;
			Assert.AreEqual (100, pool.Size);

			pool.OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
			Assert.AreEqual (PoolOverflowBehavior.EXCEPTION, pool.OverflowBehavior);

			pool.OverflowBehavior = PoolOverflowBehavior.WARNING;
			Assert.AreEqual (PoolOverflowBehavior.WARNING, pool.OverflowBehavior);

			pool.OverflowBehavior = PoolOverflowBehavior.IGNORE;
			Assert.AreEqual (PoolOverflowBehavior.IGNORE, pool.OverflowBehavior);

			pool.InflationType = PoolInflationType.DOUBLE;
			Assert.AreEqual (PoolInflationType.DOUBLE, pool.InflationType);

			pool.InflationType = PoolInflationType.INCREMENT;
			Assert.AreEqual (PoolInflationType.INCREMENT, pool.InflationType);
		}

		[Test]
		public void TestAdd()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
				Assert.AreEqual (a + 1, pool.Available);
			}
		}

		[Test]
		public void TestAddList()
		{
			pool.Size = 4;
			ClassToBeInjected[] list = new ClassToBeInjected[pool.Size];
			for (int a = 0; a < pool.Size; a++)
			{
				list[a] = new ClassToBeInjected ();
			}
			pool.Add (list);
			Assert.AreEqual (pool.Size, pool.Available);
		}

		[Test]
		public void TestGetInstance()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.Size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.Available);
				ClassToBeInjected instance = pool.GetInstance () as ClassToBeInjected;
				Assert.IsNotNull (instance);
				Assert.IsInstanceOf<ClassToBeInjected> (instance);
				Assert.AreEqual (a - 1, pool.Available);
			}
		}

		[Test]
		public void TestReturnInstance()
		{
			pool.Size = 4;
			Stack stack = new Stack (pool.Size);
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = 0; a < pool.Size; a++)
			{
				stack.Push(pool.GetInstance ());
			}

			Assert.AreEqual (pool.Size, stack.Count);
			Assert.AreEqual (0, pool.Available);

			for (int a = 0; a < pool.Size; a++)
			{
				pool.ReturnInstance (stack.Pop ());
			}

			Assert.AreEqual (0, stack.Count);
			Assert.AreEqual (pool.Size, pool.Available);
		}

		[Test]
		public void TestClean()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}
			pool.Clean ();
			Assert.AreEqual (0, pool.Available);
		}

		[Test]
		public void TestPoolOverflowException()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.Size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.Available);
				pool.GetInstance ();
			}

			TestDelegate testDelegate = delegate()
			{
				pool.GetInstance();
			};
			PoolException ex = Assert.Throws<PoolException> (testDelegate);
			Assert.AreEqual (PoolExceptionType.OVERFLOW, ex.type);
		}

		[Test]
		public void TestOverflowWithoutException()
		{
			pool.Size = 4;
			pool.OverflowBehavior = PoolOverflowBehavior.IGNORE;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.Size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.Available);
				pool.GetInstance ();
			}

			TestDelegate testDelegate = delegate()
			{
				object shouldBeNull = pool.GetInstance();
				Assert.IsNull(shouldBeNull);
			};
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestPoolTypeMismatchException()
		{
			pool.Size = 4;
			pool.Add (new ClassToBeInjected ());

			TestDelegate testDelegate = delegate()
			{
				pool.Add(new InjectableDerivedClass());
			};
			PoolException ex = Assert.Throws<PoolException> (testDelegate);
			Assert.AreEqual (PoolExceptionType.TYPE_MISMATCH, ex.type);
		}

		[Test]
		public void TestRemoveFromPool()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.Size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.Available);
				ClassToBeInjected instance = pool.GetInstance () as ClassToBeInjected;
				pool.Remove (instance);
			}

			Assert.AreEqual (0, pool.Available);
		}

		[Test]
		public void TestRemoveList1()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			ClassToBeInjected[] removalList = new ClassToBeInjected[3];
			for (int a = 0; a < pool.Size - 1; a++)
			{
				removalList [a] = new ClassToBeInjected ();
			}
			pool.Remove (removalList);
			Assert.AreEqual (1, pool.Available);
		}

		[Test]
		public void TestRemoveList2()
		{
			pool.Size = 4;
			for (int a = 0; a < pool.Size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			ClassToBeInjected[] removalList = new ClassToBeInjected[3];
			for (int a = 0; a < pool.Size - 1; a++)
			{
				removalList [a] = pool.GetInstance () as ClassToBeInjected;
			}
			pool.Remove (removalList);
			Assert.AreEqual (1, pool.Available);
		}

		[Test]
		public void TestRemovalException()
		{
			pool.Size = 4;
			pool.Add (new ClassToBeInjected ());
			TestDelegate testDelegate = delegate()
			{
				pool.Remove (new InjectableDerivedClass ());
			};
			PoolException ex = Assert.Throws<PoolException> (testDelegate);
			Assert.AreEqual (PoolExceptionType.TYPE_MISMATCH, ex.type);
		}

		[Test]
		public void TestReleaseOfPoolable()
		{
			Pool<PooledInstance> anotherPool = new Pool<PooledInstance>();

			anotherPool.Size = 4;
			anotherPool.Add (new PooledInstance ());
			PooledInstance instance = anotherPool.GetInstance () as PooledInstance;
			instance.someValue = 42;
			Assert.AreEqual (42, instance.someValue);
			anotherPool.ReturnInstance (instance);
			Assert.AreEqual (0, instance.someValue);
		}

		//Double is default
		[Test]
		public void TestAutoInflationDouble()
		{
			pool.InstanceProvider = new TestInstanceProvider ();

			ClassToBeInjected instance1 = pool.GetInstance () as ClassToBeInjected;
			Assert.IsNotNull (instance1);
			Assert.AreEqual (1, pool.InstanceCount);	//First call creates one instance
			Assert.AreEqual (0, pool.Available);		//Nothing available

			ClassToBeInjected instance2 = pool.GetInstance () as ClassToBeInjected;
			Assert.IsNotNull (instance2);
			Assert.AreNotSame (instance1, instance2);
			Assert.AreEqual (2, pool.InstanceCount);	//Second call doubles. We have 2
			Assert.AreEqual (0, pool.Available);		//Nothing available

			ClassToBeInjected instance3 = pool.GetInstance () as ClassToBeInjected;
			Assert.IsNotNull (instance3);
			Assert.AreEqual (4, pool.InstanceCount);	//Third call doubles. We have 4
			Assert.AreEqual (1, pool.Available);		//One allocated. One available.

			ClassToBeInjected instance4 = pool.GetInstance () as ClassToBeInjected;
			Assert.IsNotNull (instance4);
			Assert.AreEqual (4, pool.InstanceCount);	//Fourth call. No doubling since one was available.
			Assert.AreEqual (0, pool.Available);

			ClassToBeInjected instance5 = pool.GetInstance () as ClassToBeInjected;
			Assert.IsNotNull (instance5);
			Assert.AreEqual (8, pool.InstanceCount);	//Fifth call. Double to 8.
			Assert.AreEqual (3, pool.Available);		//Three left unallocated.
		}

		[Test]
		public void TestAutoInflationIncrement()
		{
			pool.InstanceProvider = new TestInstanceProvider ();
			pool.InflationType = PoolInflationType.INCREMENT;

			int testCount = 10;

			Stack stack = new Stack();

			//Calls should simply increment. There will never be unallocated
			for (int a = 0; a < testCount; a++)
			{
				ClassToBeInjected instance = pool.GetInstance () as ClassToBeInjected;
				Assert.IsNotNull (instance);
				Assert.AreEqual (a + 1, pool.InstanceCount);
				Assert.AreEqual (0, pool.Available);
				stack.Push (instance);
			}

			//Now return the instances
			for (int a = 0; a < testCount; a++)
			{
				ClassToBeInjected instance = stack.Pop () as ClassToBeInjected;
				pool.ReturnInstance (instance);

				Assert.AreEqual (a + 1, pool.Available, "This one");
				Assert.AreEqual (testCount, pool.InstanceCount, "Or this one");
			}
		}
	}

	class PooledInstance : IPoolable
	{
		public int someValue = 0;

		public void Restore ()
		{
			someValue = 0;
		}
	}

	class TestInstanceProvider : IInstanceProvider
	{
		public object GetInstance<T>()
		{
			return Activator.CreateInstance (typeof (T));
		}

		public object GetInstance(Type key)
		{
			return Activator.CreateInstance (key);
		}
	}
}

