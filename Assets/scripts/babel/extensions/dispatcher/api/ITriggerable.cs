using System;

namespace babel.extensions.dispatcher.api
{
	public interface ITriggerable
	{
		void Trigger<T>(object data);
		void Trigger(object key, object data);
	}
}

