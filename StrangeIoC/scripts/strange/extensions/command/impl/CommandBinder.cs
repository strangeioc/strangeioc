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
 * Note that CommandBinder also features sequencing. By default, CommandBinder fires all
 * Commands in parallel. If your binding specifies `InSequence()`, commands will run serially,
 * with the option of suspending the chain at any time.
 * 
 * Example bindings:

		Bind("someEvent").To<SomeCommand>(); //Works, but poor form to use strings. Use the next example instead

		Bind(EventMap.SOME_EVENT).To<SomeCommand>(); //Make it a constant

		Bind(ContextEvent.START).To<StartCommand>().Once(); //Destroy the binding immediately after a single use

		Bind(EventMap.END_GAME_EVENT).To<FirstCommand>().To<SecondCommand>().To<ThirdGCommand>().InSequence();

 * 
 * See Command for details on asynchronous Commands and cancelling sequences.
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

		/// Tracker for parallel commands in progress
		protected HashSet<ICommand> activeCommands = new HashSet<ICommand>();

		/// Tracker for sequences in progress
		protected Dictionary<ICommand, ICommandBinding> activeSequences = new Dictionary<ICommand, ICommandBinding> ();

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
				if (binding.isSequence)
				{
					next (binding, data, 0);
				}
				else
				{
					object[] values = binding.value as object[];
					int aa = values.Length + 1;
					for (int a = 0; a < aa; a++)
					{
						next (binding, data, a);
					}
				}
			}
		}

		private void next(ICommandBinding binding, object data, int depth)
		{
			object[] values = binding.value as object[];
			if (depth < values.Length)
			{
				Type cmd = values [depth] as Type;
				ICommand command = invokeCommand (cmd, binding, data, depth);
				ReleaseCommand (command);
			}
			else
			{
				if (binding.isOneOff)
				{
					Unbind (binding);
				}
			}
		}

		virtual protected ICommand invokeCommand(Type cmd, ICommandBinding binding, object data, int depth)
		{
			ICommand command = createCommand (cmd, data);
			command.sequenceId = depth;
			trackCommand (command, binding);
			executeCommand (command);
			return command;
		}

		virtual protected ICommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ICommand> ().To (cmd);
			ICommand command = injectionBinder.GetInstance<ICommand> () as ICommand;

			if (command == null)
			{
				string msg = "A Command ";
				if (data != null)
				{
					msg += "tied to data " + data.ToString ();
				}
				msg += " could not be instantiated.\nThis might be caused by a null pointer during instantiation or failing to override Execute (generally you shouldn't have constructor code in Commands).";
				throw new CommandException(msg, CommandExceptionType.BAD_CONSTRUCTOR);
			}

			command.data = data;
			injectionBinder.Unbind<ICommand> ();
			return command;
		}

		protected void trackCommand (ICommand command, ICommandBinding binding)
		{
			if (binding.isSequence)
			{
				activeSequences [command] = binding;
			}
			else
			{
				activeCommands.Add(command);
			}
		}

		protected void executeCommand(ICommand command)
		{
			if (command == null)
			{
				return;
			}
			command.Execute ();
		}

		public virtual void Stop(object key)
		{
			if (key is ICommand && activeSequences.ContainsKey(key as ICommand))
			{
				removeSequence (key as ICommand);
			}
			else
			{
				ICommandBinding binding = GetBinding (key) as ICommandBinding;
				if (binding != null)
				{
					if (activeSequences.ContainsValue (binding))
					{
						foreach(KeyValuePair<ICommand, ICommandBinding> sequence in activeSequences)
						{
							if (sequence.Value == binding)
							{
								ICommand command = sequence.Key;
								removeSequence (command);
							}
						}
					}
				}
			}
		}

		public void ReleaseCommand (ICommand command)
		{
			if (command.retain == false)
			{
				if (activeCommands.Contains(command))
				{
					activeCommands.Remove (command);
				}
				else if (activeSequences.ContainsKey(command))
				{
					ICommandBinding binding = activeSequences [command];
					object data = command.data;
					activeSequences.Remove (command);
					next (binding, data, command.sequenceId + 1);
				}
			}
		}

		private void removeSequence(ICommand command)
		{
			if (activeSequences.ContainsKey (command))
			{
				command.Cancel();
				activeSequences.Remove (command);
			}
		}

		public bool Trigger<T>(object data)
		{
			return Trigger (typeof(T), data);
		}

		public bool Trigger(object key, object data)
		{
			ReactTo(key, data);
			return true;
		}

		new public virtual  ICommandBinding Bind<T> ()
		{
			return base.Bind<T> () as ICommandBinding;
		}

		new public virtual ICommandBinding Bind (object value)
		{
			return base.Bind (value) as ICommandBinding;
		}
	}
}

