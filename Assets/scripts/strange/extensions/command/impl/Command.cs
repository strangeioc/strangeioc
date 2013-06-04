/**
 * @class strange.extensions.command.impl.Command
 * 
 * Commands are where you place your business logic.
 * 
 * In the MVCSContext setup, commands are mapped to IEvents. 
 * The firing of a specific event on the global event bus triggers 
 * the instantiation, injection and execution of any Command(s) bound to that event.
 * 
 * By default, commands are cleaned up immediately on completion of the `Execute()` method.
 * For asynchronous Commands (e.g., calling a service and awaiting a response),
 * call `Retain()` at the top of your `Execute()` method, which will prevent
 * premature cleanup. But remember, having done so it is your responsipility
 * to call `Release()` once the Command is complete.
 */

using System;
using strange.extensions.command.api;
using strange.extensions.injector.api;

namespace strange.extensions.command.impl
{
	public class Command : ICommand
	{
		/// Back reference to the CommandBinder that instantiated this Commmand
		[Inject]
		public ICommandBinder commandBinder{ get; set;}

		/// The InjectionBinder for this Context
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

		public bool retain
		{
			get
			{
				return _retain;
			}
		}
	}
}

