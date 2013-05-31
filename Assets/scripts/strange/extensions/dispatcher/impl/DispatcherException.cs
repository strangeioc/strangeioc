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

		public DispatcherException(string message, DispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

