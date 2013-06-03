using System;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.mediation.impl
{
	public class ViewWithDispatcher : View
	{
		[Inject]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

