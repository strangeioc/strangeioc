using System;
using babel.extensions.context.api;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.extensions.mediation.impl
{
	public class MediatorWithDispatcher : View
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

