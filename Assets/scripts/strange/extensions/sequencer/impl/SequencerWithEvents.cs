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
		
		override protected ISequenceCommand createCommand(object cmd, object data)
		{
			injectionBinder.Bind<ISequenceCommand> ().To (cmd);
			if (data is TmEvent)
			{
				injectionBinder.Bind<TmEvent> ().AsValue(data);
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

