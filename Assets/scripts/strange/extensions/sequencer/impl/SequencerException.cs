/**
 * @class strange.extensions.sequencer.impl.SequencerException
 * 
 * An exception thrown by the Sequencer.
 */

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

		/// Constructs a SequencerException with a message and SequencerExceptionType
		public SequencerException(string message, SequencerExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

