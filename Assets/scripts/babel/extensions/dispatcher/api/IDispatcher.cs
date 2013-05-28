using System;

namespace babel.extensions.dispatcher.api
{
	public interface IDispatcher
	{
		void Dispatch (object eventType);
		void Dispatch (object eventType, object data);
	}
}

