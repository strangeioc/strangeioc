/**
 * @class strange.extensions.injector.impl.InjectionException
 * 
 * An exception thrown by the Injection system.
 */

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

		/// Constructs an InjectionException with a message and InjectionExceptionType
		public InjectionException(string message, InjectionExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

