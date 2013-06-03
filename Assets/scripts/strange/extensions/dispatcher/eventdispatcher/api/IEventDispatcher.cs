/**
 * @interface strange.extensions.dispatcher.eventdispatcher.api.IEventDispatcher
 * 
 * Interface for allowing a client to register as an observer.
 * 
 * EventDispatcher allows a client to register as an observer. Whenever the 
 * Dispatcher executes a `Dispatch()`, observers will be notified of any event
 * (Key) for which they have registered.
 * 
 * EventDispatcher dispatches TmEvents.
 * 
 * The EventDispatcher is the only Dispatcher currently released with Strange
 * (though by separating EventDispatcher from Dispatcher I'm obviously
 * signalling that I don't think it's the only possible one).
 * 
 * @see strange.extensions.dispatcher.eventdispatcher.impl.TmEvent
 */

using System;
using strange.extensions.dispatcher.api;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public interface IEventDispatcher : IDispatcher
	{
		IEventBinding Bind(object key);

		/// Add an observer with exactly one argument to this Dispatcher
		void addListener(object evt, EventCallback callback);

		/// Add an observer with exactly no arguments to this Dispatcher
		void addListener(object evt, EmptyCallback callback);

		/// Remove a previously registered observer with exactly one argument from this Dispatcher
		void removeListener(object evt, EventCallback callback);

		/// Remove a previously registered observer with exactly no arguments from this Dispatcher
		void removeListener(object evt, EmptyCallback callback);

		/// Returns true if the provided observer is already registered
		bool hasListener(object evt, EventCallback callback);

		/// Returns true if the provided observer is already registered
		bool hasListener(object evt, EmptyCallback callback);

		/// By passing true, an observer with exactly one argument will be added to this Dispatcher
		void updateListener(bool toAdd, object evt, EventCallback callback);

		/// By passing true, an observer with exactly no arguments will be added to this Dispatcher
		void updateListener(bool toAdd, object evt, EmptyCallback callback);
	}
}

