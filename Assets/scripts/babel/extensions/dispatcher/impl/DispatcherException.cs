using System;
using babel.extensions.dispatcher.api;

namespace babel.extensions.dispatcher.impl
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

