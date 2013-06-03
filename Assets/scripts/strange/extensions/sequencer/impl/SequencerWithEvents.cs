/**
 * @class strange.extensions.sequencer.impl.SequencerWithEvents
 * 
 * This variation on Sequencer injects TmEvents.
 * 
 * It's the version used in MVCSContext.
 */

using System;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.sequencer.api;

namespace strange.extensions.sequencer.impl
{
	public class SequencerWithEvents : Sequencer
	{
		public SequencerWithEvents ()
		{
		}

		/// Instantiate and Inject the command, incling a TmEvent to data.
		override protected ISequenceCommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ISequenceCommand> ().To (cmd);
			if (data is TmEvent)
			{
				injectionBinder.Bind<TmEvent> ().ToValue(data);
			}
			ISequenceCommand command = injectionBinder.GetInstance<ISequenceCommand> () as ISequenceCommand;
			command.data = data;
			if (data is TmEvent)
			{
				injectionBinder.Unbind<TmEvent> ();
			}
			injectionBinder.Unbind<ISequenceCommand> ();
			return command;
		}
	}
}

