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
 * @interface strange.extensions.dispatcher.eventdispatcher.api.IEventBinding
 * 
 * Binding interface for EventDispatcher.
 * 
 * EventBindings technically allow any Key, but require either an 
 * EmptyCallback (no arguments) or an EventCallback (one argument).
 * 
 * The IEvent only accepts strings as keys, so in the standard MVCSContext
 * setup, your EventBinder keys should also be strings.
 * 
 * @see strange.extensions.dispatcher.eventdispatcher.api.IEvent
 */

using System;
using strange.framework.api;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	/// Delegate for adding a listener with a single argument
	public delegate void EventCallback(IEvent payload);

	/// Delegate for adding a listener with a no arguments
	public delegate void EmptyCallback();

	public interface IEventBinding : IBinding
	{
		/// Retrieve the type of the provided callback
		EventCallbackType typeForCallback (EventCallback callback);

		/// Retrieve the type of the provided callback
		EventCallbackType typeForCallback (EmptyCallback callback);

		new IEventBinding Key (object key);
		IEventBinding To (EventCallback callback);
		IEventBinding To (EmptyCallback callback);
	}
}

