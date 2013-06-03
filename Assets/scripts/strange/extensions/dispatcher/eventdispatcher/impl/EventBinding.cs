/**
 * @interface strange.extensions.dispatcher.eventdispatcher.impl.EventBinding
 * 
 * A Binding for the EventDispatcher.
 * 
 * EventBindings technically allow any Key, but require either an 
 * EmptyCallback (no arguments) or an EventCallback (one argument).
 * 
 * The TmEvent only accepts strings as keys, so in the standard MVCSContext
 * setup, your EventBinder keys should also be strings.
 *
 * @see strange.extensions.dispatcher.eventdispatcher.impl.TmEvent
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using strange.extensions.dispatcher.api;
using strange.extensions.dispatcher.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class EventBinding : Binding, IEventBinding
	{
		private Dictionary<Delegate, EventCallbackType> callbackTypes;

		public EventBinding () : this(null)
		{
		}

		public EventBinding (strange.framework.impl.Binder.BindingResolver resolver) : base(resolver)
		{
			keyConstraint = BindingConstraintType.ONE;
			valueConstraint = BindingConstraintType.MANY;
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

