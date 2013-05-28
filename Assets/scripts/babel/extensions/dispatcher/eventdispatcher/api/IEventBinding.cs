using System;
using babel.framework.api;

namespace babel.extensions.dispatcher.eventdispatcher.api
{
	public delegate void EventCallback(object payload);
	public delegate void EmptyCallback();

	public interface IEventBinding : IBinding
	{
		EventCallbackType typeForCallback (EventCallback callback);
		EventCallbackType typeForCallback (EmptyCallback callback);

		new IEventBinding Key (object key);
		IEventBinding To (EventCallback callback);
		IEventBinding To (EmptyCallback callback);
	}
}

