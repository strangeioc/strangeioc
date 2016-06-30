using System;
using NUnit.Framework;
using strange.extensions.sequencer.api;
using strange.extensions.sequencer.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestSequenceCommand
	{

		[Test]
		public void TestSuccessfulExecute ()
		{
			ISequenceCommand command = new SequenceCommandWithExecute ();
			TestDelegate testDelegate = delegate()
			{
				command.Execute();
			};
			Assert.DoesNotThrow (testDelegate);
		}

		[Test]
		public void TestRetainRelease ()
		{
			ISequenceCommand command = new SequenceCommandWithExecute ();
			Assert.IsFalse (command.retain);
			command.Retain ();
			Assert.IsTrue (command.retain);
			command.Release ();
			Assert.IsFalse (command.retain);
		}

		[Test]
		public void TestCancel()
		{
			ISequenceCommand command = new SequenceCommandWithExecute ();
			command.Cancel ();
			Assert.IsTrue (command.cancelled);
		}

		[Test]
		public void TestSequenceId()
		{
			ISequenceCommand command = new SequenceCommandWithExecute ();
			command.sequenceId = 42;
			Assert.AreEqual (42, command.sequenceId);
		}
	}
}

