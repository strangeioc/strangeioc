/*
 * Copyright 2015 StrangeIoC
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
 * @class strange.extensions.mediation.SignalMediationBinder
 * 
 * This subclass of the MediationBinder provides support for
 * the ListensTo shortcut.
 */

using System;
using System.Reflection;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using strange.extensions.reflector.api;
using strange.extensions.signal.impl;
using UnityEngine;

namespace strange.extensions.mediation
{
	public class SignalMediationBinder : MediationBinder
	{

		/// Adds a Mediator to a View
		protected override object CreateMediator(IView view, Type mediatorType)
		{
			object mediator = base.CreateMediator(view, mediatorType);
			if (mediator is IMediator)
			{
				HandleDelegates(mediator, mediatorType, true);
			}
			return mediator;
		}

		/// Manage Delegates, then remove the Mediator from a View
		protected override IMediator DestroyMediator(IView view, Type mediatorType)
		{
			IMediator mediator = base.DestroyMediator(view, mediatorType);
			if (mediator != null)
			{
				HandleDelegates(mediator, mediatorType, false);
			}

			return mediator;
		}

		/// Determine whether to add or remove ListensTo delegates
		protected void HandleDelegates(object mono, Type mediatorType, bool toAdd)
		{
			IReflectedClass reflectedClass = injectionBinder.injector.reflector.Get(mediatorType);

			//GetInstance Signals and add listeners
			foreach (var pair in reflectedClass.attrMethods)
			{
				if (pair.Value is ListensTo)
				{
					ListensTo attr = (ListensTo)pair.Value;
					ISignal signal = (ISignal)injectionBinder.GetInstance(attr.type);
					if (toAdd)
						AssignDelegate(mono, signal, pair.Key);
					else
						RemoveDelegate(mono, signal, pair.Key);
				}
			}
		}

		/// Remove any existing ListensTo Delegates
		protected void RemoveDelegate(object mediator, ISignal signal, MethodInfo method)
		{
			if (signal.GetType().BaseType.IsGenericType) //e.g. Signal<T>, Signal<T,U> etc.
			{
				Delegate toRemove = Delegate.CreateDelegate(signal.listener.GetType(), mediator, method);
				signal.listener = Delegate.Remove(signal.listener,toRemove);
			}
			else
			{
				((Signal)signal).RemoveListener((Action)Delegate.CreateDelegate(typeof(Action), mediator, method)); //Assign and cast explicitly for Type == Signal case
			}
		}

		/// Apply ListensTo delegates
		protected void AssignDelegate(object mediator, ISignal signal, MethodInfo method)
		{
			if (signal.GetType().BaseType.IsGenericType)
			{
				var toAdd = Delegate.CreateDelegate(signal.listener.GetType(), mediator, method); //e.g. Signal<T>, Signal<T,U> etc.
				signal.listener = Delegate.Combine(signal.listener, toAdd);
			}
			else
			{
				((Signal)signal).AddListener((Action)Delegate.CreateDelegate(typeof(Action), mediator, method)); //Assign and cast explicitly for Type == Signal case
			}
		}
	}
}
