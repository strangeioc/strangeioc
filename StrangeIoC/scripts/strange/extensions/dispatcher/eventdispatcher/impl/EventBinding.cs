/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @interface strange.extensions.dispatcher.eventdispatcher.impl.EventBinding
 * 
 * A Binding for the EventDispatcher.
 * 
 * EventBindings technically allow any Key, but require either an 
 * EmptyCallback (no arguments) or an EventCallback (one argument).
 *
 * @see strange.extensions.dispatcher.eventdispatcher.api.IEvent
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

