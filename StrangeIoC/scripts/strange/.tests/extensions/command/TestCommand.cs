
using System;
using NUnit.Framework;
using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestCommand
	{

		[Test]
		public void TestMissingExecute ()
		{
			ICommand command = new CommandWithoutExecute ();
			TestDelegate testDelegate = delegate()
			{
				command.Execute();
			};
			CommandException ex = Assert.Throws<CommandException> (testDelegate);
			Assert.That (ex.type == CommandExceptionType.EXECUTE_OVERRIDE);
		}

		[Test]
		public void TestSuccessfulExecute ()
		{
			ICommand command = new CommandWithExecute ();
			TestDelegate testDelegate = delegate()
			{
				command.Execute();
			};
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestRetainRelease ()
		{
			ICommand command = new CommandWithExecute ();
			Assert.IsFalse (command.retain);
			command.Retain ();
			Assert.IsTrue (command.retain);
			command.Release ();
			Assert.IsFalse (command.retain);
		}
	}
}
