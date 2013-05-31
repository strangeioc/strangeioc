using System;

namespace strange.extensions.dispatcher.api
{
	public interface ITriggerProvider
	{
		void AddTriggerable(ITriggerable target);
		void RemoveTriggerable(ITriggerable target);
	}
}

