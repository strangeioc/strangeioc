using System;
using System.Collections.Generic;
using System.Reflection;
using babel.extensions.dispatcher.api;
using babel.extensions.dispatcher.impl;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.framework.api;
using babel.framework.impl;

namespace babel.extensions.dispatcher.eventdispatcher.impl
{
	public class EventBinding : Binding, IEventBinding
	{
		private Dictionary<Delegate, EventCallbackType> callbackTypes;

		public EventBinding () : base()
		{
			callbackTypes = new Dictionary<Delegate, EventCallbackType> ();
		}

		public EventBinding (babel.framework.impl.Binder.BindingResolver resolver) : base(resolver)
		{
			callbackTypes = new Dictionary<Delegate, EventCallbackType> ();
		}

		public EventCallbackType typeForCallback(EmptyCallback callback)
		{ 
			if (callbackTypes.ContainsKey (callback)) 
			{
				return callbackTypes [callback];
			}
			return EventCallbackType.NOT_FOUND;
		}

		public EventCallbackType typeForCallback(EventCallback callback)
		{ 
			if (callbackTypes.ContainsKey (callback)) 
			{
				return callbackTypes [callback];
			}
			return EventCallbackType.NOT_FOUND;
		}

		new public IEventBinding Key(object key)
		{
			return base.Key (key) as IEventBinding;
		}

		public IEventBinding To(EventCallback value)
		{
			base.To (value);
			storeMethodType(value as Delegate);
			return this;
		}

		public IEventBinding To(EmptyCallback value)
		{
			base.To (value);
			storeMethodType(value as Delegate);
			return this;
		}

		new public IEventBinding To(object value)
		{
			base.To (value);
			storeMethodType(value as Delegate);
			return this;
		}

		override public void RemoveValue(object value)
		{
			base.RemoveValue (value);
			callbackTypes.Remove (value as Delegate);
		}

		private void storeMethodType(Delegate value)
		{
			if (value == null)
			{
				throw new DispatcherException ("EventDispatcher can't map something that isn't a delegate'", DispatcherExceptionType.ILLEGAL_CALLBACK_HANDLER);
			}
			MethodInfo methodInfo = value.Method;
			int argsLen = methodInfo.GetParameters ().Length;
			switch(argsLen)
			{
				case 0:
					callbackTypes[value] = EventCallbackType.NO_ARGUMENTS;
					break;
				case 1:
					callbackTypes[value] = EventCallbackType.ONE_ARGUMENT;
					break;
				default:
					throw new DispatcherException ("Event callbacks must have either one or no arguments", DispatcherExceptionType.ILLEGAL_CALLBACK_HANDLER);
			}
		}
	}
}

