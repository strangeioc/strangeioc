/**
 * @interface strange.extensions.dispatcher.api.ITriggerProvider
 * 
 * Interface for declaring a class capable of triggering an ITriggerable class.
 * 
 * Objects implementing a TriggerProvider declare themselves able to
 * provide triggering to any ITriggerable. The contract specifies that 
 * TriggerProvider will pass events on to the Triggerable class.
 * This allows notifications, such as TmEvents, to pass through 
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
	}
}

