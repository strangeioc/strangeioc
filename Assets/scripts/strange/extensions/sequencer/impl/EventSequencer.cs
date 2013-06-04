/**
 * @class strange.extensions.sequencer.impl.EventSequencer
 * 
 * This variation on Sequencer injects IEvents.
 * 
 * It's the version used in MVCSContext.
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

