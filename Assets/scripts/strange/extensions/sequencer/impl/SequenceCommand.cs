using System;
using strange.extensions.command.impl;
using strange.extensions.injector.api;
using strange.extensions.sequencer.api;

namespace strange.extensions.sequencer.impl
{
	public class SequenceCommand : ISequenceCommand
	{
		[Inject]
		public ISequencer sequencer{ get; set;}

		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}
		
		public object data{ get; set; }

		private bool _retain = false;
		private bool _cancelled = false;

		public SequenceCommand ()
		{
		}

		public void BreakSequence ()
		{
			if (sequencer != null)
			{
				sequencer.BreakSequence (this);
			}
		}

		virtual public void Execute ()
		{
			throw new SequencerException ("You must override the Execute method in every SequenceCommand", SequencerExceptionType.EXECUTE_OVERRIDE);
		}

		public void Retain ()
		{
			_retain = true;
		}

		public void Release ()
		{
			_retain = false;
			if (sequencer != null)
			{
				sequencer.ReleaseCommand (this);
			}
		}

		virtual public void Dispose ()
		{
		}

		public void Cancel ()
		{
			_cancelled = true;
		}

		public bool retain 
		{
			get 
			{
				return _retain;
			}
		}

		public bool cancelled 
		{
			get 
			{
				return _cancelled;
			}
		}

		public int sequenceId{ get; set; }
	}
}

