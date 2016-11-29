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
 * @class strange.extensions.command.impl.EventCommand
 * 
 * Subclass of Command with injections for dispatcher and events.
 * 
 * EventCommand extends Command to provide access to EventDispatcher as the common system bus.
 * Commands which extend Event Command will automatically inject the source IEvent.
 */

using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.command.impl
{
	public abstract class EventCommand : Command
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

		[Inject]
		public IEvent evt{ get; set;}

		public override void Retain ()
		{
			base.Retain ();
		}

		public override void Release ()
		{
			base.Release ();
		}
	}
}

