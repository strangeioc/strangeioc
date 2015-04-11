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
 * @class strange.extensions.signal.impl.BaseSignal
 * 
 * The base class for all Signals.
 * 
 * @see strange.extensions.signal.api.IBaseSignal
 * @see strange.extensions.signal.impl.Signal
 */

using System;
using strange.extensions.signal.api;
using System.Collections.Generic;
using System.Linq;

namespace strange.extensions.signal.impl
{
	public class BaseSignal : IBaseSignal
	{
		
		/// The delegate for repeating listeners
		public event Action<IBaseSignal, object[]> BaseListener = null;

		/// The delegate for one-off listeners
		public event Action<IBaseSignal, object[]> OnceBaseListener = null;

		public void Dispatch(object[] args) 
		{ 
			if (BaseListener != null)
				BaseListener(this, args);
			if (OnceBaseListener != null)
				OnceBaseListener(this, args);
			OnceBaseListener = null;
		}

		public virtual List<Type> GetTypes() { return new List<Type>(); }

		public void AddListener(Action<IBaseSignal, object[]> callback)
		{
			BaseListener = AddUnique(BaseListener, callback);
		}

		public void AddOnce(Action<IBaseSignal, object[]> callback)
		{
			OnceBaseListener = AddUnique(OnceBaseListener, callback);
		}

		private Action<T, U> AddUnique<T,U>(Action<T, U> listeners, Action<T, U> callback)
		{
			if (listeners == null || !listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}

		public void RemoveListener(Action<IBaseSignal, object[]> callback)
		{
			if (BaseListener != null)
				BaseListener -= callback;
		}

		public virtual void RemoveAllListeners()
		{
			BaseListener = null;
			OnceBaseListener = null;
		}
	   
	}
}

