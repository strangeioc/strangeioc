/**
 * @interface strange.extensions.dispatcher.api.IDispatcher
 * 
 * A Dispatcher sends notifiations to any registered listener.
 * It represents the subject in a standard Observer pattern.
 * 
 * In MVCSContext the dispatched notification is an IEvent.
 */

using System;

namespace strange.extensions.dispatcher.api
{
	public interface IDispatcher
	{
		/// Send a notification of type eventType. No data.
		/// In MVCSContext this dispatches an IEvent.
		void Dispatch (object eventType);

		/// Send a notification of type eventType and the provided data payload.
		/// In MVCSContext this dispatches an IEvent.
		void Dispatch (object eventType, object data);
	}
}

