using System;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.extensions.mediation.impl
{
	public class ViewWithDispatcher : View
	{
		[Inject]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

