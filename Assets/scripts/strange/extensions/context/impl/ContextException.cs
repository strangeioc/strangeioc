/**
 * @class strange.extensions.context.impl.ContextException
 * 
 * An exception raised by the Context system.
 * 
 * @see strange.extensions.context.api.ContextExceptionType
 */

using System;
using strange.extensions.context.api;

namespace strange.extensions.context.impl
{
	public class ContextException : Exception
	{
		public ContextExceptionType type{ get; set;}

		public ContextException () : base()
		{
		}

		/// Constructs a ContextException with a message and ContextExceptionType
		public ContextException(string message, ContextExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

