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
		public event Action<IBaseSignal, object[]> BaseListener = delegate { };

		/// The delegate for one-off listeners
		public event Action<IBaseSignal, object[]> OnceBaseListener;

		public void Dispatch(object[] args) 
		{ 
			BaseListener(this, args);

			if(null != OnceBaseListener)
			{
				var onceBaseDelegates = OnceBaseListener;
				OnceBaseListener = null;
				onceBaseDelegates(this, args);
			}
		}

		public virtual List<Type> GetTypes() { return new List<Type>(); }

		public void AddListener(Action<IBaseSignal, object[]> callback) 
		{
			if (BaseListener.GetInvocationList().Contains(callback))
				return;

			BaseListener += callback;
		}

		public void AddOnce(Action<IBaseSignal, object[]> callback)
		{
			// Check if the delegate is not yet created
			if (null == OnceBaseListener)
				OnceBaseListener = delegate { };
			// Well, it was, so make sure the callback is unique
			else if (OnceBaseListener.GetInvocationList().Contains(callback))
				return;

			OnceBaseListener += callback;
		}

		public void RemoveListener(Action<IBaseSignal, object[]> callback) { BaseListener -= callback; }

	   
	}
}

