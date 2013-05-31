using System;
using strange.extensions.sequencer.api;

namespace strange.extensions.sequencer.impl
{
	public class SequencerException : Exception
	{
		public SequencerExceptionType type{ get; set;}

		public SequencerException () : base()
		{
		}

		public SequencerException(string message, SequencerExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

