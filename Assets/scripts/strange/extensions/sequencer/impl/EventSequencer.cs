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
 * @class strange.extensions.sequencer.impl.EventSequencer
 * 
 * @deprecated
 */

using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.sequencer.api;

namespace strange.extensions.sequencer.impl
{
	public class EventSequencer : Sequencer
	{
		public EventSequencer ()
		{
		}

		/// Instantiate and Inject the command, incling an IEvent to data.
		override protected ISequenceCommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ISequenceCommand> ().To (cmd);
			if (data is IEvent)
			{
				injectionBinder.Bind<IEvent> ().ToValue(data).ToInject(false);;
			}
			ISequenceCommand command = injectionBinder.GetInstance<ISequenceCommand> () as ISequenceCommand;
			command.data = data;
			if (data is IEvent)
			{
				injectionBinder.Unbind<IEvent> ();
			}
			injectionBinder.Unbind<ISequenceCommand> ();
			return command;
		}
	}
}

