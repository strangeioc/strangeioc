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
		public void TestMissingExecute ()
		{
			ISequenceCommand command = new SequenceCommandWithoutExecute ();
			TestDelegate testDelegate = delegate()
			{
				command.Execute();
			};
			SequencerException ex = Assert.Throws<SequencerException> (testDelegate);
			Assert.That (ex.type == SequencerExceptionType.EXECUTE_OVERRIDE);
		}

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
			Assert.IsFalse (command.Retained);
			command.Retain ();
			Assert.IsTrue (command.Retained);
			command.Release ();
			Assert.IsFalse (command.Retained);
		}

		[Test]
		public void TestCancel()
		{
			ISequenceCommand command = new SequenceCommandWithExecute ();
			command.Cancel ();
			Assert.IsTrue (command.Cancelled);
		}

		[Test]
		public void TestSequenceId()
		{
			ISequenceCommand command = new SequenceCommandWithExecute ();
			command.SequenceId = 42;
			Assert.AreEqual (42, command.SequenceId);
		}
	}
}

