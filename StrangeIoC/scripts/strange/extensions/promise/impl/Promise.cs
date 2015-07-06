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
* @class strange.extensions.promise.impl.Promise
*
* @see strange.extensions.promise.api.IPromise
*/

using System;
using System.Linq;
using strange.extensions.promise.api;
using UnityEngine;

namespace strange.extensions.promise.impl
{
	public class Promise : BasePromise, IPromise
	{
		protected event Action Listener = null;

		/// <summary>
		/// Trigger completion callbacks to all listeners.
		/// </summary>
		public void Dispatch()
		{
			if (Fulfill())
				CallListener();
			Finally();
		}

		/// <summary>
		/// Handle a callback when the Promise completes successfully.
		/// </summary>
		/// <param name="action">The callback (no arguments).</param>
		public IPromise Then(Action action)
		{
			if (Fulfilled)
			{
				action();
				Finally();
			}
			else if (Pending)
			{
				Listener = AddUnique(Listener, action);
			}

			return this;
		}

		public void RemoveListener(Action action)
		{
			if (Listener != null)
				Listener -= action;
		}
		public override void RemoveAllListeners()
		{
			base.RemoveAllListeners();
			Listener = null;
		}

		public override int ListenerCount()
		{
			return Listener == null ? 0 : Listener.GetInvocationList().Length;
		}

		private void CallListener()
		{
			if (Listener != null)
				Listener();
		}
	}

	public class Promise<T> : BasePromise, IPromise<T>
	{
		private T t;

		protected event Action<T> Listener = null;

		/// <summary>
		/// Trigger completion callbacks to all listeners
		/// </summary>
		/// <param name="t">First param.</param>
		public void Dispatch(T t)
		{
			if (Fulfill())
			{
				this.t = t;
				CallListener();
				Finally();
			}
		}

		/// <summary>
		/// Handle a callback when the Promise completes successfully.
		/// </summary>
		/// <param name="action">The callback (one argument).</param>
		public IPromise<T> Then(Action<T> action)
		{
			if (Fulfilled)
			{
				action(this.t);
				Finally();
			}
			else if (Pending)
			{
				Listener = AddUnique(Listener, action);
			}
			return this;
		}

		public void RemoveListener(Action<T> action)
		{
			if (Listener != null)
				Listener -= action;
		}
		public override void RemoveAllListeners()
		{
			base.RemoveAllListeners();
			Listener = null;
		}
		public override int ListenerCount()
		{
			return Listener == null ? 0 : Listener.GetInvocationList().Length;
		}
		private void CallListener()
		{
			if (Listener != null)
				Listener(t);
		}

	}

	public class Promise<T,U> : BasePromise, IPromise<T,U>
	{
		private T t;
		private U u;

		protected event Action<T,U> Listener = null;

		/// <summary>
		/// Trigger completion callbacks to all listeners
		/// </summary>
		/// <param name="t">First param.</param>
		/// <param name="u">Second param.</param>
		public void Dispatch(T t, U u)
		{
			if (Fulfill())
			{
				this.t = t;
				this.u = u;
				CallListener();
				Finally();
			}
		}

		/// <summary>
		/// Handle a callback when the Promise completes successfully.
		/// </summary>
		/// <param name="action">The callback (two arguments).</param>
		public IPromise<T,U> Then(Action<T,U> action)
		{
			if (Fulfilled)
			{
				action(this.t, this.u);
				Finally();
			}
			else if (Pending)
			{
				Listener = AddUnique(Listener, action);
			}
			return this;
		}

		public void RemoveListener(Action<T, U> action)
		{
			if (Listener != null)
				Listener -= action;
		}
		public override void RemoveAllListeners()
		{
			base.RemoveAllListeners();
			Listener = null;
		}
		public override int ListenerCount()
		{
			return Listener == null ? 0 : Listener.GetInvocationList().Length;
		}
		private void CallListener()
		{
			if (Listener != null)
				Listener(t, u);
		}

		protected Action<T, U> AddUnique(Action<T, U> listeners, Action<T, U> callback)
		{
			if (listeners == null || !listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

	public class Promise<T,U,V> : BasePromise, IPromise<T,U,V>
	{
		private T t;
		private U u;
		private V v;

		protected event Action<T, U, V> Listener = null;

		/// <summary>
		/// Trigger completion callbacks to all listeners
		/// </summary>
		/// <param name="t">First param.</param>
		/// <param name="u">Second param.</param>
		/// <param name="v">Third param.</param>
		public void Dispatch(T t, U u, V v)
		{
			if (Fulfill())
			{
				this.t = t;
				this.u = u;
				this.v = v;
				CallListener();
				Finally();
			}
		}

		/// <summary>
		/// Handle a callback when the Promise completes successfully.
		/// </summary>
		/// <param name="action">The callback (three arguments).</param>
		public IPromise<T,U,V> Then(Action<T, U, V> action)
		{
			if (Fulfilled)
			{
				action(this.t, this.u, this.v);
				Finally();
			}
			else if (Pending)
			{
				Listener = AddUnique(Listener, action);
			}
			return this;
		}

		public void RemoveListener(Action<T, U, V> action)
		{
			if (Listener != null)
				Listener -= action;
		}
		public override void RemoveAllListeners()
		{
			base.RemoveAllListeners();
			Listener = null;
		}
		public override int ListenerCount()
		{
			return Listener == null ? 0 : Listener.GetInvocationList().Length;
		}
		private void CallListener()
		{
			if (Listener != null)
				Listener(t, u, v);
		}

		protected Action<T, U, V> AddUnique(Action<T, U, V> listeners, Action<T, U, V> callback)
		{
			if (listeners == null || !listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}

	public class Promise<T,U,V,W> : BasePromise, IPromise<T,U,V,W>
	{
		private T t;
		private U u;
		private V v;
		private W w;

		protected event Action<T, U, V, W> Listener = null;

		/// <summary>
		/// Trigger completion callbacks to all listeners
		/// </summary>
		/// <param name="t">First param.</param>
		/// <param name="u">Second param.</param>
		/// <param name="v">Third param.</param>
		/// <param name="w">Fourth param.</param>
		public void Dispatch(T t, U u, V v, W w)
		{
			if (Fulfill())
			{
				this.t = t;
				this.u = u;
				this.v = v;
				this.w = w;
				CallListener();
				Finally();
			}
		}

		/// <summary>
		/// Handle a callback when the Promise completes successfully.
		/// </summary>
		/// <param name="action">The callback (four arguments).</param>
		public IPromise<T, U, V,W> Then(Action<T, U, V, W> action)
		{
			if (Fulfilled)
			{
				action(t, u, v, w);
				Finally();
			}
			else if (Pending)
			{
				Listener = AddUnique(Listener, action);
			}
			return this;
		}

		public void RemoveListener(Action<T, U, V, W> action)
		{
			if (Listener != null)
				Listener -= action;
		}
		public override void RemoveAllListeners()
		{
			base.RemoveAllListeners();
			Listener = null;
		}
		public override int ListenerCount()
		{
			return Listener == null ? 0 : Listener.GetInvocationList().Length;
		}
		private void CallListener()
		{
			if (Listener != null)
				Listener(t, u, v, w);
		}

		protected Action<T, U, V, W> AddUnique(Action<T, U, V, W> listeners, Action<T, U, V, W> callback)
		{
			if (listeners == null || !listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}
	}
}
