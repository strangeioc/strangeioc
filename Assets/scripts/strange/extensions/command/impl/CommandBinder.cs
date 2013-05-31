using System;
using System.Collections.Generic;
using strange.extensions.command.api;
using strange.extensions.dispatcher.api;
using strange.extensions.injector.api;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.command.impl
{
	public class CommandBinder : Binder, ICommandBinder, ITriggerable
	{
		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}

		protected Dictionary<ICommand, ICommand> activeCommands = new Dictionary<ICommand, ICommand>();

		public CommandBinder ()
		{
		}

		public override IBinding GetRawBinding ()
		{
			return new CommandBinding(resolver);
		}

		public void ReactTo (object trigger)
		{
			ReactTo (trigger, null);
		}
		
		public void ReactTo(object trigger, object data)
		{
			ICommandBinding binding = GetBinding (trigger) as ICommandBinding;
			if (binding != null)
			{
				object[] values = binding.value as object[];
				int aa = values.Length;
				for (int a = 0; a < aa; a++)
				{
					Type cmd = values [a] as Type;
					invokeCommand (cmd, binding, data, 0);
				}
			}
		}

		virtual protected void invokeCommand(Type cmd, ICommandBinding binding, object data, int depth)
		{
			ICommand command = createCommand (cmd, data);
			trackCommand (command);
			executeCommand (command);
			if (binding.isOneOff)
			{
				Unbind (binding);
			}
			ReleaseCommand (command);
		}

		virtual protected ICommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ICommand> ().To (cmd);
			ICommand command = injectionBinder.GetInstance<ICommand> () as ICommand;
			command.data = data;
			injectionBinder.Unbind<ICommand> ();
			return command;
		}

		private void trackCommand (ICommand command)
		{
			activeCommands [command] = command;
		}

		private void executeCommand(ICommand command)
		{
			if (command == null)
			{
				return;
			}
			command.Execute ();
		}

		public void ReleaseCommand (ICommand command)
		{
			if (command.retain == false)
			{
				if (activeCommands.ContainsKey(command))
				{
					activeCommands.Remove (command);
				}
			}
		}

		private void failIf(bool condition, string message, CommandExceptionType type)
		{
			if (condition)
			{
				throw new CommandException(message, type);
			}
		}

		public void Trigger<T>(object data)
		{
			Trigger (typeof(T), data);
		}

		public void Trigger(object key, object data)
		{
			ReactTo(key, data);
		}

		new public ICommandBinding Bind<T> ()
		{
			return base.Bind<T> () as ICommandBinding;
		}

		new public ICommandBinding Bind (object value)
		{
			return base.Bind (value) as ICommandBinding;
		}
	}
}

