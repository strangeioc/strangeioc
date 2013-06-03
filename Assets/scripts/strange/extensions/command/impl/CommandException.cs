/**
 * @class strange.extensions.command.impl.CommandException
 * 
 * An exception raised by the Command system.
 * 
 * @see strange.extensions.context.api.CommandExceptionType
 */

using System;
using strange.extensions.command.api;

namespace strange.extensions.command.impl
{
	public class CommandException : Exception
	{
		public CommandExceptionType type{ get; set;}

		public CommandException () : base()
		{
		}

		/// Constructs a CommandException with a message and CommandExceptionType
		public CommandException(string message, CommandExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

