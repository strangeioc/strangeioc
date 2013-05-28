using System;

namespace babel.extensions.dispatcher.api
{
	public interface ITriggerProvider
	{
		void AddTriggerable(ITriggerable target);
		void RemoveTriggerable(ITriggerable target);
	}
}

