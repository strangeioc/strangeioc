/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.extensions.command.impl.CommandBinder
 * 
 * A Binder that triggers the instantiation of Commands.
 * 
 * Commands are where the logic of your application belongs.
 * These Commands typically focus on a single function, such as
 * adding a View, requesting a service, reading from or saving to a model.
 * 
 * The act of binding events to Commands means that code needn't know
 * anything about an event recipient, or even how the event will be used.
 * For example, a Mediator might send out an event that two View objects
 * collided. A Command would then determine that the result of that event
 * was to Destroy both objects, tell a ScoreKeeper model to change the
 * score and request a message be sent to the server. Whether that
 * example means one Command or three is up to your coding preference...
 * CommandBinder can trigger one Command or multiple Commands off the
 * same event.
 * 
 * Note that Strange also a features a Sequencer. CommandBinder fires all
 * Commands in parallel, while Sequencer runs them serially, with the
 * option of suspending the chain at any time.
 * 
 * Example bindings:

		Bind("someEvent").To<SomeCommand>(); //Works, but poor form to use strings. Use the next example instead

		Bind(EventMap.SOME_EVENT).To<SomeCommand>(); //Make it a constant

		Bind(ContextEvent.START).To<StartCommand>().Once(); //Destroy the binding immediately after a single use

 * 
 * See Command for details on asynchronous Commands.
 */

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

		virtual public void ReactTo (object trigger)
		{
			ReactTo (trigger, null);
		}
		
		virtual public void ReactTo(object trigger, object data)
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

