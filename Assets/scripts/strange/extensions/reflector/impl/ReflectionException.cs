using System;
using strange.extensions.reflector.api;

namespace strange.extensions.reflector.impl
{
	public class ReflectionException : Exception
	{
		public ReflectionExceptionType type{ get; set;}

		public ReflectionException() : base()
		{
		}

		public ReflectionException(string message, ReflectionExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

