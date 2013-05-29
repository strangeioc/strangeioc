using System;
using babel.extensions.context.api;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.extensions.mediation.impl
{
	public class MediatorWithDispatcher : Mediator
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

