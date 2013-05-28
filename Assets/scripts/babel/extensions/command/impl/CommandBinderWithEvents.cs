using System;
using babel.extensions.command.api;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.extensions.command.impl
{
	public class CommandBinderWithEvents : CommandBinder
	{
		public CommandBinderWithEvents ()
		{
		}

		override protected ICommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ICommand> ().To (cmd);
			if (data is TmEvent)
			{
				injectionBinder.Bind<TmEvent>().AsValue(data);
			}
			ICommand command = injectionBinder.GetInstance<ICommand> () as ICommand;
			command.data = data;
			if (data is TmEvent)
			{
				injectionBinder.Unbind<TmEvent>();
			}
			injectionBinder.Unbind<ICommand> ();
			return command;
		}
	}
}

