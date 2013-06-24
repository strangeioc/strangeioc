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
 * @interface strange.extensions.command.api.ICommandBinder
 * 
 * Interface for a Binder that triggers the instantiation of Commands.
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
 * Note that CommandBinder also a features sequencing. By default, CommandBinder fires all
 * Commands in parallel. If your binding specifies `InSequence()`,  commands will run serially,
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
using strange.extensions.injector.api;
using strange.framework.api;

namespace strange.extensions.command.api
{
	public interface ICommandBinder : IBinder
	{

		/// Trigger a key that unlocks one or more Commands
		void ReactTo (object trigger);

		/// Trigger a key that unlocks one or more Commands and provide a data injection to that Command
		void ReactTo (object trigger, object data);

		/// Release a previously retained Command.
		/// By default, a Command is garbage collected at the end of its `Execute()` method. 
		/// But the Command can be retained for asynchronous calls.
		void ReleaseCommand(ICommand command);

		/// Called to halt execution of a currently running command group
		void Stop(object key);

		/// Bind a trigger Key by generic Type
		new ICommandBinding Bind<T>();

		/// Bind a trigger Key by value
		new ICommandBinding Bind(object value);
	}
}

