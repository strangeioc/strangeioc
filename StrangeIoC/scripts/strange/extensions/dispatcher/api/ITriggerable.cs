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
 * @interface strange.extensions.dispatcher.api.ITriggerable
 * 
 * Interface for declaring a class capable of being triggered by a provided key and/or name.
 * 
 * Objects implementing ITriggerable can register with objects implementing
 * ITriggerProvider. The contract specifies that TriggerProvider will
 * pass events on to the Triggerable class. This allows notifications,
 * such as IEvents to pass through the event bus and trigger other binders.
 * 
 * @see strange.extensions.dispatcher.api.ITriggerProvider
 */

using System;

namespace strange.extensions.dispatcher.api
{
	public interface ITriggerable
	{
		/// Cause this ITriggerable to access any provided Key in its Binder by the provided generic and data.
		/// <returns>false if the originator should abort dispatch</returns>
		bool Trigger<T>(object data);

		/// Cause this ITriggerable to access any provided Key in its Binder by the provided key and data.
		/// <returns>false if the originator should abort dispatch</returns>
		bool Trigger(object key, object data);
	}
}

