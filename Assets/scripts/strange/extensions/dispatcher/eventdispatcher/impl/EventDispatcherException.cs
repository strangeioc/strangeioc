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

		public EventDispatcherException(string message, EventDispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

