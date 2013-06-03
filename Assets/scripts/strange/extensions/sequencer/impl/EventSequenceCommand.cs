/**
 * @class strange.extensions.sequencer.impl.EventSequenceCommand
 * 
 * A subclass of SequenceCommand which injects a TmEvent and a Dispatcher.
 * 
 * EventSequenceCommand extends SequenceCommand to provide access 
 * to EventDispatcher as the common system bus. SequenceCommands
 * which extend EventSequenceCommand will automatically inject 
 * the source TMEvent.
 */

using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.sequencer.impl;

namespace strange.extensions.sequencer.impl
{
	public class EventSequenceCommand : SequenceCommand
	{
		/// The context-wide Event bus
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

		/// The injected TmEvent
		[Inject]
		public TmEvent evt{ get; set;}
	}
}

