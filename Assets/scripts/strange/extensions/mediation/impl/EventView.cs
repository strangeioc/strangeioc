/**
 * @class strange.extensions.mediation.impl.EventView
 * 
 * Injects a local event bus into this View. Intended
 * for local communication between the View and its
 * Mediator.
 * 
 * Caution: we recommend against injecting the context-wide
 * dispatcher into a View.
 */

using System;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.mediation.impl
{
	public class EventView : View
	{
		[Inject]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

