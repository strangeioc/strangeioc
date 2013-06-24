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
 * 
 * Calling `Fail()` will terminate any sequence in which the Command is operating, but
 * has no effect on Commands operating in parallel.
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

		public bool cancelled{ get; set;}

		public int sequenceId{ get; set; }

		protected bool _retain = false;

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

		public void Fail()
		{
			if (commandBinder != null)
			{
				commandBinder.Stop (this);
			}
		}

		public void Cancel()
		{
			cancelled = true;
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

