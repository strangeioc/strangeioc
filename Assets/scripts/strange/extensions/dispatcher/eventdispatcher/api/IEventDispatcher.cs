using System;
using strange.extensions.dispatcher.api;

namespace strange.extensions.dispatcher.eventdispatcher.api
{
	public interface IEventDispatcher : IDispatcher
	{
		IEventBinding Bind(object key);

		void addListener(object evt, EventCallback callback);
		void addListener(object evt, EmptyCallback callback);

		void removeListener(object evt, EventCallback callback);
		void removeListener(object evt, EmptyCallback callback);

		bool hasListener(object evt, EventCallback callback);
		bool hasListener(object evt, EmptyCallback callback);

		void updateListener(bool toAdd, object evt, EventCallback callback);
		void updateListener(bool toAdd, object evt, EmptyCallback callback);
	}
}

