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

