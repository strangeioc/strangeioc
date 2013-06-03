/**
 * @class strange.extensions.command.impl.CommandBinderWithEvents
 * 
 * 
 */

using System;
using strange.extensions.command.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.extensions.command.impl
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
				injectionBinder.Bind<TmEvent>().ToValue(data).ToInject(false);
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

