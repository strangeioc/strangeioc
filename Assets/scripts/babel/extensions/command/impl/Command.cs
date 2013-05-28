using System;
using babel.extensions.command.api;
using babel.extensions.injector.api;

namespace babel.extensions.command.impl
{
	public class Command : ICommand
	{
		[Inject]
		public ICommandBinder commandBinder{ get; set;}

		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}
		
		public object data{ get; set;}

		private bool _retain = false;

		public Command ()
		{
		}

		virtual public void Execute()
		{
			throw new CommandException ("You must override the Execute method in every Command", CommandExceptionType.EXECUTE_OVERRIDE);
		}

		public void Retain()
		{
			_retain = true;
		}

		public void Release()
		{
			_retain = false;
			if (commandBinder != null)
			{
				commandBinder.ReleaseCommand (this);
			}
		}

		virtual public void Dispose()
		{
		}

		public bool retain
		{
			get
			{
				return _retain;
			}
		}
	}
}

