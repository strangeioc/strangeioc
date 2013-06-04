/**
 * @class strange.extensions.command.impl.EventCommand
 * 
 * Subclass of Command with injections for dispatcher and events.
 * 
 * EventCommand extends Command to provide access to EventDispatcher as the common system bus.
 * Commands which extend Event Command will automatically inject the source IEvent.
 */

using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.command.impl;

namespace strange.extensions.command.impl
{
	public class EventCommand : Command
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

		[Inject]
		public IEvent evt{ get; set;}
	}
}

