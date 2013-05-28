using System;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.extensions.dispatcher.eventdispatcher.impl
{
	public class EventDispatcherException : Exception
	{
		public EventDispatcherExceptionType type{ get; set;}

		public EventDispatcherException() : base()
		{
		}

		public EventDispatcherException(string message, EventDispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

