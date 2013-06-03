using System;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public enum EventDispatcherExceptionType
	{
		/// Indicates that an event was fired with null as the key
		EVENT_KEY_NULL,
		
		/// Indicates that the type of Event in the call and the type of Event in the payload don't match.
		EVENT_TYPE_MISMATCH
	}
}

