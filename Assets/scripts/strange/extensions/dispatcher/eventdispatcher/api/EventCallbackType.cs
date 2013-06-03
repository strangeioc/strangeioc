using System;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public enum EventCallbackType
	{
		/// Indicates an EventCallback with no arguments
		NO_ARGUMENTS,
		/// Indicates an EventCallback with one argument
		ONE_ARGUMENT,
		/// Indicates no matching EventCallback could be found
		NOT_FOUND
	}
}

