/**
 * @interface strange.extensions.dispatcher.api.ITriggerable
 * 
 * Interface for declaring a class capable of being triggered by a provided key and/or name.
 * 
 * Objects implementing ITriggerable can register with objects implementing
 * ITriggerProvider. The contract specifies that TriggerProvider will
 * pass events on to the Triggerable class. This allows notifications,
 * such as TmEvents to pass through the event bus and trigger other binders.
 * 
 * @see strange.extensions.dispatcher.api.ITriggerProvider
 */

using System;

namespace strange.extensions.dispatcher.api
{
	public interface ITriggerable
	{
		/// Cause this ITriggerable to access any provided Key in its Binder by the provided generic and data.
		void Trigger<T>(object data);

		/// Cause this ITriggerable to access any provided Key in its Binder by the provided key and data.
		void Trigger(object key, object data);
	}
}

