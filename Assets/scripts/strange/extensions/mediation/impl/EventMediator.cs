/**
 * @class strange.extensions.mediation.impl.EventMediator
 * 
 * A Mediator which injects an IEventDispatcher. This is the
 * class for your Mediators to extend if you're using MVCSContext.
 */

using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.mediation.impl
{
	public class EventMediator : Mediator
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

