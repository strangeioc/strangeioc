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
			Assert.AreEqual (0, pool.size);

			pool.size = 100;
			Assert.AreEqual (100, pool.size);

			pool.overflowBehavior = PoolOverflowBehavior.EXCEPTION;
			Assert.AreEqual (PoolOverflowBehavior.EXCEPTION, pool.overflowBehavior);

			pool.overflowBehavior = PoolOverflowBehavior.WARNING;
			Assert.AreEqual (PoolOverflowBehavior.WARNING, pool.overflowBehavior);

			pool.overflowBehavior = PoolOverflowBehavior.IGNORE;
			Assert.AreEqual (PoolOverflowBehavior.IGNORE, pool.overflowBehavior);

			pool.inflationType = PoolInflationType.DOUBLE;
			Assert.AreEqual (PoolInflationType.DOUBLE, pool.inflationType);

			pool.inflationType = PoolInflationType.INCREMENT;
			Assert.AreEqual (PoolInflationType.INCREMENT, pool.inflationType);
		}

		[Test]
		public void TestAdd()
		{
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
				Assert.AreEqual (a + 1, pool.available);
			}
		}

		[Test]
		public void TestAddList()
		{
			pool.size = 4;
			ClassToBeInjected[] list = new ClassToBeInjected[pool.size];
			for (int a = 0; a < pool.size; a++)
			{
				list[a] = new ClassToBeInjected ();
			}
			pool.Add (list);
			Assert.AreEqual (pool.size, pool.available);
		}

		[Test]
		public void TestGetInstance()
		{
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.available);
				ClassToBeInjected instance = pool.GetInstance ();
				Assert.IsNotNull (instance);
				Assert.IsInstanceOf<ClassToBeInjected> (instance);
				Assert.AreEqual (a - 1, pool.available);
			}
		}

		[Test]
		public void TestReturnInstance()
		{
			pool.size = 4;
			Stack stack = new Stack (pool.size);
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = 0; a < pool.size; a++)
			{
				stack.Push(pool.GetInstance ());
			}

			Assert.AreEqual (pool.size, stack.Count);
			Assert.AreEqual (0, pool.available);

			for (int a = 0; a < pool.size; a++)
			{
				pool.ReturnInstance (stack.Pop ());
			}

			Assert.AreEqual (0, stack.Count);
			Assert.AreEqual (pool.size, pool.available);
		}

		[Test]
		public void TestClean()
		{
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}
			pool.Clean ();
			Assert.AreEqual (0, pool.available);
		}

		[Test]
		public void TestPoolOverflowException()
		{
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.available);
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
			pool.size = 4;
			pool.overflowBehavior = PoolOverflowBehavior.IGNORE;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.available);
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
			pool.size = 4;
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
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			for (int a = pool.size; a > 0; a--)
			{
				Assert.AreEqual (a, pool.available);
				ClassToBeInjected instance = pool.GetInstance ();
				pool.Remove (instance);
			}

			Assert.AreEqual (0, pool.available);
		}

		[Test]
		public void TestRemoveList1()
		{
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			ClassToBeInjected[] removalList = new ClassToBeInjected[3];
			for (int a = 0; a < pool.size - 1; a++)
			{
				removalList [a] = new ClassToBeInjected ();
			}
			pool.Remove (removalList);
			Assert.AreEqual (1, pool.available);
		}

		[Test]
		public void TestRemoveList2()
		{
			pool.size = 4;
			for (int a = 0; a < pool.size; a++)
			{
				pool.Add (new ClassToBeInjected ());
			}

			ClassToBeInjected[] removalList = new ClassToBeInjected[3];
			for (int a = 0; a < pool.size - 1; a++)
			{
				removalList [a] = pool.GetInstance ();
			}
			pool.Remove (removalList);
			Assert.AreEqual (1, pool.available);
		}

		[Test]
		public void TestRemovalException()
		{
			pool.size = 4;
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

			anotherPool.size = 4;
			anotherPool.Add (new PooledInstance ());
			PooledInstance instance = anotherPool.GetInstance ();
			instance.someValue = 42;
			Assert.AreEqual (42, instance.someValue);
			anotherPool.ReturnInstance (instance);
			Assert.AreEqual (0, instance.someValue);
		}

		//Double is default
		[Test]
		public void TestAutoInflationDouble()
		{
			pool.instanceProvider = new TestInstanceProvider ();

			ClassToBeInjected instance1 = pool.GetInstance ();
			Assert.IsNotNull (instance1);
			Assert.AreEqual (1, pool.instanceCount);	//First call creates one instance
			Assert.AreEqual (0, pool.available);		//Nothing available

			ClassToBeInjected instance2 = pool.GetInstance ();
			Assert.IsNotNull (instance2);
			Assert.AreNotSame (instance1, instance2);
			Assert.AreEqual (2, pool.instanceCount);	//Second call doubles. We have 2
			Assert.AreEqual (0, pool.available);		//Nothing available

			ClassToBeInjected instance3 = pool.GetInstance ();
			Assert.IsNotNull (instance3);
			Assert.AreEqual (4, pool.instanceCount);	//Third call doubles. We have 4
			Assert.AreEqual (1, pool.available);		//One allocated. One available.

			ClassToBeInjected instance4 = pool.GetInstance ();
			Assert.IsNotNull (instance4);
			Assert.AreEqual (4, pool.instanceCount);	//Fourth call. No doubling since one was available.
			Assert.AreEqual (0, pool.available);

			ClassToBeInjected instance5 = pool.GetInstance ();
			Assert.IsNotNull (instance5);
			Assert.AreEqual (8, pool.instanceCount);	//Fifth call. Double to 8.
			Assert.AreEqual (3, pool.available);		//Three left unallocated.
		}

		[Test]
		public void TestAutoInflationIncrement()
		{
			pool.instanceProvider = new TestInstanceProvider ();
			pool.inflationType = PoolInflationType.INCREMENT;

			int testCount = 10;

			Stack stack = new Stack();

			//Calls should simply increment. There will never be unallocated
			for (int a = 0; a < testCount; a++)
			{
				ClassToBeInjected instance = pool.GetInstance ();
				Assert.IsNotNull (instance);
				Assert.AreEqual (a + 1, pool.instanceCount);
				Assert.AreEqual (0, pool.available);
				stack.Push (instance);
			}

			//Now return the instances
			for (int a = 0; a < testCount; a++)
			{
				ClassToBeInjected instance = stack.Pop () as ClassToBeInjected;
				pool.ReturnInstance (instance);

				Assert.AreEqual (a + 1, pool.available, "This one");
				Assert.AreEqual (testCount, pool.instanceCount, "Or this one");
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

		public void Retain()
		{
		}

		public void Release()
		{
		}

		public bool retain { get; set; }
	}

	class TestInstanceProvider : IInstanceProvider
	{
		public T GetInstance<T>()
		{
			object instance = Activator.CreateInstance (typeof (T));
			T retv = (T) instance;
			return retv;
		}

		public object GetInstance(Type key)
		{
			return Activator.CreateInstance (key);
		}
	}
}

