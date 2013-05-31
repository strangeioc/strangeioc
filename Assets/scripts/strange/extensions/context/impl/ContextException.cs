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

		public ContextException(string message, ContextExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

