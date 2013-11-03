
using System;
using System.Collections.Generic;
using NUnit.Framework;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.extensions.pool.impl;
using strange.extensions.pool.api;
using strange.framework.api;

namespace strange.unittests
{
	[TestFixture()]
	public class TestCommandPooling
	{
		private ICommandBinder commandBinder;
		private IPooledCommandBinder pooledCommandBinder;
		private IInjectionBinder injectionBinder;


		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder ();
			injectionBinder.Bind<IInjectionBinder>().Bind<IInstanceProvider>().ToValue(injectionBinder);
			injectionBinder.Bind<ICommandBinder> ().To<CommandBinder> ();

			commandBinder = injectionBinder.GetInstance<ICommandBinder> () as ICommandBinder;
			pooledCommandBinder = commandBinder as IPooledCommandBinder;
		}

		[Test]
		public void TestCommandGetsReused()
		{
			commandBinder.Bind (SomeEnum.ONE).To<MarkablePoolCommand> ();


			IPool pool = pooledCommandBinder.getPool<MarkablePoolCommand> ();
			//MarkablePoolCommand cmd = pool.GetInstance () as MarkablePoolCommand;
			//MarkablePoolCommand cmd = pool.GetInstance () as MarkablePoolCommand;

			for (int a = 0; a < 10; a++)
			{
				commandBinder.ReactTo (SomeEnum.ONE);
				Assert.AreEqual (a+1, MarkablePoolCommand.incrementValue);
				Assert.AreEqual (1, pool.InstanceCount);
			}
		}

		[Test]
		public void ttttttt()
		{
		
			//var Customer = new { FirstName = "John", LastName = "Doe" };
			//var customerList = MakeList(Customer);

			//customerList.Add(new { FirstName = "Bill", LastName = "Smith" });

			var explicitCommand = new MarkablePoolCommand ();
			var explicitList = MakeList (explicitCommand);
			explicitList.Add (new MarkablePoolCommand ());

			Type tc = typeof(MarkablePoolCommand);
			var cmd = Activator.CreateInstance (tc);
			var list = MakeList (cmd);
			   

			list.Add (new MarkablePoolCommand ());
		}

		public static Pool<T> MakeList<T>(T itemOftype)
		{
			Pool<T> newList = new Pool<T>();
			newList.PoolType = itemOftype.GetType ();
			return newList;
		}   


		/// Issue #33. For Pooling of Commands, Release should null out all injected params.
		[Test]
		public void TestReleasesInjections()
		{
			ICommand command = new CommandWithInjection ();
			Assert.IsFalse (command.retain);
			command.Retain ();
			Assert.IsTrue (command.retain);
			command.Release ();
			Assert.IsFalse (command.retain);
		}
	}
}

