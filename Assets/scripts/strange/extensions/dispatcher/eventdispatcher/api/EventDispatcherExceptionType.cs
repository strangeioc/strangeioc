using System;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public enum EventDispatcherExceptionType
	{
		/// Indicates that the type of Event in the call and the type of Event in the payload don't match.
		EVENT_TYPE_MISMATCH
	}
}

