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
 * @interface strange.extensions.dispatcher.api.ITriggerProvider
 * 
 * Interface for declaring a class capable of triggering an ITriggerable class.
 * 
 * Objects implementing a TriggerProvider declare themselves able to
 * provide triggering to any ITriggerable. The contract specifies that 
 * TriggerProvider will pass events on to the Triggerable class.
 * This allows notifications, such as IEvents, to pass through 
 * the event bus and trigger other binders.
 * 
 * @see strange.extensions.dispatcher.api.ITriggerable
 */

using System;

namespace strange.extensions.dispatcher.api
{
	public interface ITriggerProvider
	{
		/// Register a Triggerable client with this provider.
		void AddTriggerable(ITriggerable target);

		/// Remove a previously registered Triggerable client from this provider.
		void RemoveTriggerable(ITriggerable target);

		/// Count of the current number of trigger clients.
		int Triggerables{ get;}
	}
}

