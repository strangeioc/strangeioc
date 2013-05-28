using System;
using babel.extensions.dispatcher.eventdispatcher.impl;
using babel.extensions.sequencer.api;

namespace babel.extensions.sequencer.impl
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

