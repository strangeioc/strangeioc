using System;
using babel.extensions.command.api;

namespace babel.extensions.command.impl
{
	public class CommandException : Exception
	{
		public CommandExceptionType type{ get; set;}

		public CommandException () : base()
		{
		}

		public CommandException(string message, CommandExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

