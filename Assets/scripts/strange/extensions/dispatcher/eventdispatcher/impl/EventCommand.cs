/**
 * EventCommand extends Command to provide access to EventDispatcher as the common system bus.
 * Commands which extend Event Command will automatically inject the source TMEvent.
 */

using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class EventCommand : strange.extensions.command.impl.Command
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

		[Inject]
		public TmEvent evt{ get; set;}
	}
}

