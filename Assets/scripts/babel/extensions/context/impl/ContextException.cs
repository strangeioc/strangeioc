using System;
using babel.extensions.context.api;

namespace babel.extensions.context.impl
{
	public class ContextException : Exception
	{
		public ContextExceptionType type{ get; set;}

		public ContextException () : base()
		{
		}

		public ContextException(string message, ContextExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

