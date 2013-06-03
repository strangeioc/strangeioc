/**
 * @class strange.extensions.dispatcher.impl.DispatcherException
 * 
 * An exception thrown by the Dispatcher system.
 */

using System;
using strange.extensions.dispatcher.api;

namespace strange.extensions.dispatcher.impl
{
	public class DispatcherException : Exception
	{
		public DispatcherExceptionType type{ get; set;}

		public DispatcherException() : base()
		{
		}

		/// Constructs a DispatcherException with a message and DispatcherExceptionType
		public DispatcherException(string message, DispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

