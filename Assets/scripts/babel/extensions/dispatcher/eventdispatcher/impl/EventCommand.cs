/**
 * EventCommand extends Command to provide access to EventDispatcher as the common system bus.
 * Commands which extend Event Command will automatically inject the source TMEvent.
 */

using System;
using babel.extensions.context.api;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.command.api;
using babel.extensions.command.impl;

namespace babel.extensions.dispatcher.eventdispatcher.impl
{
	public class EventCommand : babel.extensions.command.impl.Command
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

		[Inject]
		public TmEvent evt{ get; set;}
	}
}

