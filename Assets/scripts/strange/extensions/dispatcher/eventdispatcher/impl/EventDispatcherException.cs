/**
 * @class strange.extensions.dispatcher.eventdispatcher.impl.EventDispatcherException
 * 
 * An exception thrown by the EventDispatcher system.
 */

using System;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class EventDispatcherException : Exception
	{
		public EventDispatcherExceptionType type{ get; set;}

		public EventDispatcherException() : base()
		{
		}

		/// Constructs an EventDispatcherException with a message and EventDispatcherExceptionType
		public EventDispatcherException(string message, EventDispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

