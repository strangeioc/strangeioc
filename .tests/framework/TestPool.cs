using System;
using NUnit.Framework;
using strange.framework.api;
using strange.framework.impl;
using System.Collections;

namespace strange.unittests
{
	[TestFixture()]
	public class TestPool
	{
		Pool pool;


		[SetUp]
		public void Setup()
		{
			pool = new Pool ();
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
			Assert.That (ex.type == PoolExceptionType.OVERFLOW);
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
			Assert.That (ex.type == PoolExceptionType.TYPE_MISMATCH);
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
			Assert.That (ex.type == PoolExceptionType.TYPE_MISMATCH);
		}

		[Test]
		public void TestReleaseOfPoolable()
		{
			pool.Size = 4;
			pool.Add (new PooledInstance ());
			PooledInstance instance = pool.GetInstance () as PooledInstance;
			instance.someValue = 42;
			Assert.AreEqual (42, instance.someValue);
			pool.ReturnInstance (instance);
			Assert.AreEqual (0, instance.someValue);
		}
	}

	class PooledInstance : IPoolable
	{
		public int someValue = 0;

		public void Release ()
		{
			someValue = 0;
		}
	}
}

