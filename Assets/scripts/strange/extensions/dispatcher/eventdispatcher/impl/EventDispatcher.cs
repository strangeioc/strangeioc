/**
 * @class strange.extensions.dispatcher.eventdispatcher.impl.EventDispatcher
 * 
 * A Dispatcher that uses TmEvent to send messages.
 * 
 * Whenever the Dispatcher executes a `Dispatch()`, observers will be 
 * notified of any event (Key) for which they have registered.
 * 
 * EventDispatcher dispatches TmEvents.
 * 
 * The EventDispatcher is the only Dispatcher currently released with Strange
 * (though by separating EventDispatcher from Dispatcher I'm obviously
 * signalling that I don't think it's the only possible one).
 * 
 * EventDispatcher is both an ITriggerProvider and an ITriggerable.
 * 
 * @see strange.extensions.dispatcher.eventdispatcher.impl.TmEvent
 * @see strange.extensions.dispatcher.api.ITriggerProvider
 * @see strange.extensions.dispatcher.api.ITriggerable
 */

using System;
using System.Collections.Generic;
using strange.framework.api;
using strange.framework.impl;
using strange.extensions.dispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class EventDispatcher : Binder, IEventDispatcher, ITriggerProvider, ITriggerable
	{
		/// The list of clients that will be triggered as a consequence of an Event firing.
		protected ITriggerable[] triggerClients;

		public EventDispatcher ()
		{
		}

		override public IBinding GetRawBinding()
		{
			return new EventBinding (resolver);
		}

		new public IEventBinding Bind(object key)
		{
			return base.Bind (key) as IEventBinding;
		}

		public void Dispatch (object eventType)
		{
			Dispatch (eventType, null);
		}

		public void Dispatch (object eventType, object data)
		{
			//Scrub the data to make eventType and data conform if possible
			if (eventType == null)
			{
				throw new EventDispatcherException("Attempt to Dispatch to null.\ndata: " + data, EventDispatcherExceptionType.EVENT_KEY_NULL);
			}
			else if (eventType is TmEvent)
			{
				//Client provided a full-formed event
				data = eventType;
				eventType = (data as TmEvent).type;
			}
			else if (data == null)
			{
				//Client provided just an event ID. Create an event for injection
				data = new TmEvent(eventType, this, null);
			}
			else if (data is TmEvent)
			{
				//Client provided both an evertType and a full-formed TmEvent
				(data as TmEvent).type = eventType;
			}
			else
			{
				//Client provided an eventType and some data which is not a TmEvent.
				data = new TmEvent(eventType, this, data);
			}
			
			if (triggerClients != null)
			{
				int aa = triggerClients.Length;
				for (int a = 0; a < aa; a++)
				{
					triggerClients[a].Trigger(eventType, data);
				}
			}
			
			IEventBinding binding = GetBinding (eventType) as IEventBinding;
			if (binding == null)
			{
				return;
			}

			object[] callbacks = binding.value as object[];
			
			if (callbacks == null)
			{
				return;
			}

			int bb = callbacks.Length;
			for(int b = 0; b < bb; b++)
			{
				object callback = callbacks [b];
				object[] parameters = null;
				if (callback is EventCallback)
				{
					parameters = new object[1];
					parameters [0] = data;
					EventCallback evtCb = callback as EventCallback;

					try
					{
						evtCb.DynamicInvoke (parameters);
					}
					catch
					{
						throw new EventDispatcherException ("The target callback is attempting an illegal cast. EventDispatcher expects callbacks to cast the payload as TmEvent.\nTarget class:" + evtCb.Target, EventDispatcherExceptionType.TARGET_INVOCATION);
					}
				}
				else if (callback is EmptyCallback)
				{
					parameters = new object[0];
					EmptyCallback emptyCb = callback as EmptyCallback;
					emptyCb.DynamicInvoke (parameters);
				}
			}
		}

		public void addListener(object evt, EventCallback callback)
		{
			IBinding binding = GetBinding (evt);
			if (binding == null)
			{
				Bind (evt).To (callback);
			}
			else
			{
				binding.To (callback);
			}
		}

		public void addListener(object evt, EmptyCallback callback)
		{
			IBinding binding = GetBinding (evt);
			if (binding == null)
			{
				Bind (evt).To (callback);
			}
			else
			{
				binding.To (callback);
			}
		}

		public void removeListener(object evt, EventCallback callback)
		{
			IBinding binding = GetBinding (evt);
			RemoveValue (binding, callback);
		}

		public void removeListener(object evt, EmptyCallback callback)
		{
			IBinding binding = GetBinding (evt);
			RemoveValue (binding, callback);
		}

		public bool hasListener(object evt, EventCallback callback)
		{
			IEventBinding binding = GetBinding (evt) as IEventBinding;
			if (binding == null)
			{
				return false;
			}
			return binding.typeForCallback (callback) != EventCallbackType.NOT_FOUND;
		}

		public bool hasListener(object evt, EmptyCallback callback)
		{
			IEventBinding binding = GetBinding (evt) as IEventBinding;
			if (binding == null)
			{
				return false;
			}
			return binding.typeForCallback (callback) != EventCallbackType.NOT_FOUND;
		}

		public void updateListener(bool toAdd, object evt, EventCallback callback)
		{
			if (toAdd)
			{
				addListener (evt, callback);
			}
			else
			{
				removeListener (evt, callback);
			}
		}

		public void updateListener(bool toAdd, object evt, EmptyCallback callback)
		{
			if (toAdd)
			{
				addListener (evt, callback);
			}
			else
			{
				removeListener (evt, callback);
			}
		}

		public void AddTriggerable(ITriggerable target)
		{
			if (triggerClients == null)
			{
				triggerClients = new ITriggerable[1];
			}
			else
			{
				ITriggerable[] tempList = triggerClients;
				int len = tempList.Length;
				triggerClients = new ITriggerable[len + 1];
				tempList.CopyTo (triggerClients, 0);
			}
			triggerClients [triggerClients.Length - 1] = target;
		}

		public void RemoveTriggerable(ITriggerable target)
		{
			int aa = triggerClients.Length;
			for (int a = 0; a < aa; a++)
			{
				if (triggerClients[a] == target)
				{
					spliceValueAt(a, triggerClients);
					break;
				}
			}
		}

		public void Trigger<T>(object data)
		{
			Trigger (typeof(T), data);
		}

		public void Trigger(object key, object data)
		{
			Dispatch(key, data);
		}
	}
}

