/**
 * @interface strange.extensions.dispatcher.eventdispatcher.api.IEvent
 * 
 * The interface for an event sent by the EventDispatcher
 */

using System;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public interface IEvent
	{
		/// The Event key
		object type{ get; set;}

		/// The IEventDispatcher that sent the event
		IEventDispatcher target{ get; set;}

		/// An arbitrary data payloaf
		object data{ get; set;}
	}
}

