using System;
using strange.extensions.injector.api;

namespace strange.extensions.injector.impl
{
	public class InjectionException : Exception
	{
		public InjectionExceptionType type{ get; set;}

		public InjectionException() : base()
		{
		}

		public InjectionException(string message, InjectionExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

