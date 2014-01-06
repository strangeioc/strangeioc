/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @interface strange.extensions.dispatcher.eventdispatcher.api.IEventDispatcher
 * 
 * Interface for allowing a client to register as an observer.
 * 
 * EventDispatcher allows a client to register as an observer. Whenever the 
 * Dispatcher executes a `Dispatch()`, observers will be notified of any event
 * (Key) for which they have registered.
 * 
 * EventDispatcher dispatches IEvents.
 * 
 * The EventDispatcher is the only Dispatcher currently released with Strange
 * (though by separating EventDispatcher from Dispatcher I'm obviously
 * signalling that I don't think it's the only possible one).
 * 
 * @see strange.extensions.dispatcher.eventdispatcher.api.IEvent
 */

using System;
using strange.extensions.dispatcher.api;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public interface IEventDispatcher : IDispatcher
	{
		IEventBinding Bind(object key);

		/// Add an observer with exactly one argument to this Dispatcher
		void AddListener(object evt, EventCallback callback);

		/// Add an observer with exactly no arguments to this Dispatcher
		void AddListener(object evt, EmptyCallback callback);

		/// Remove a previously registered observer with exactly one argument from this Dispatcher
		void RemoveListener(object evt, EventCallback callback);

		/// Remove a previously registered observer with exactly no arguments from this Dispatcher
		void RemoveListener(object evt, EmptyCallback callback);

		/// Returns true if the provided observer is already registered
		bool HasListener(object evt, EventCallback callback);

		/// Returns true if the provided observer is already registered
		bool HasListener(object evt, EmptyCallback callback);

		/// By passing true, an observer with exactly one argument will be added to this Dispatcher
		void UpdateListener(bool toAdd, object evt, EventCallback callback);

		/// By passing true, an observer with exactly no arguments will be added to this Dispatcher
		void UpdateListener(bool toAdd, object evt, EmptyCallback callback);

		/// Allow a previously retained event to be returned to its pool
		void ReleaseEvent(IEvent evt);
	}
}

