using System;
using System.Collections.Generic;
using babel.framework.api;
using babel.framework.impl;
using babel.extensions.dispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.extensions.dispatcher.eventdispatcher.impl
{
	public class EventDispatcher : Binder, IEventDispatcher, ITriggerProvider
	{
		
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
			if (eventType is string)
			{
				if (data == null)
				{
					//Client provided just an event ID. Create an event for injection
					data = new TmEvent(eventType as String, this, null);
				}
				else if (data is TmEvent)
				{
					string type = (data as TmEvent).type;
					if (String.IsNullOrEmpty(type))
					{
						(data as TmEvent).type = (string)eventType;
					}
					else if ((data as TmEvent).type != (eventType as string))
					{
						throw new EventDispatcherException("EventDispatcher won't accept an event where the eventType differs from the payload Event's type. This condition can arise if you relay an event without unpacking it's data.\neventType: " + eventType + "\npayload event type: " + (data as TmEvent).type, EventDispatcherExceptionType.EVENT_TYPE_MISMATCH);
					}
				}
				else
				{
					data = new TmEvent(eventType as String, this, data);
				}
			}
			else if (eventType is TmEvent)
			{
				data = eventType;
				eventType = (data as TmEvent).type;
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
					evtCb.DynamicInvoke (parameters);
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
	}
}

