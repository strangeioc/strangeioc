/**
 * @class strange.extensions.sequencer.impl.EventSequenceCommand
 * 
 * A subclass of SequenceCommand which injects an IEvent and a Dispatcher.
 * 
 * EventSequenceCommand extends SequenceCommand to provide access 
 * to EventDispatcher as the common system bus. SequenceCommands
 * which extend EventSequenceCommand will automatically inject 
 * the source IEvent.
 */

using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.sequencer.impl;

namespace strange.extensions.sequencer.impl
{
	public class EventSequenceCommand : SequenceCommand
	{
		/// The context-wide Event bus
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

		/// The injected IEvent
		[Inject]
		public IEvent evt{ get; set;}
	}
}

