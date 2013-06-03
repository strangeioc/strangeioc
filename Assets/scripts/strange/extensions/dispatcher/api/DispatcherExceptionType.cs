using System;

namespace strange.extensions.dispatcher.api
{
	public enum DispatcherExceptionType
	{
		/// Injector Factory not found
		NULL_FACTORY,

		/// Callback must be a Delegate with zero or one argument
		ILLEGAL_CALLBACK_HANDLER
	}
}

