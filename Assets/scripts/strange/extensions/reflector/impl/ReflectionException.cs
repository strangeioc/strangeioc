/**
 * @class strange.extensions.reflector.impl.ReflectionException
 * 
 * An exception thrown by the Reflector.
 */

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

		/// Constructs a ReflectionException with a message and ReflectionExceptionType
		public ReflectionException(string message, ReflectionExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

