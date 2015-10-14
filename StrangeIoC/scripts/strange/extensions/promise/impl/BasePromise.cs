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
 * @class strange.extensions.promise.impl.BasePromise
 *
 * @see strange.extensions.promise.api.IPromise
 */

using System;
using System.Linq;
using strange.extensions.promise.api;

namespace strange.extensions.promise.impl
{
	public abstract class BasePromise : IBasePromise
	{
		private event Action<float> OnProgress;
		private event Action<Exception> OnFail;
		private event Action OnFinally;
		private Exception exception;

		public PromiseState State { get; protected set; }

		public enum PromiseState
		{
			Fulfilled,
			Failed,
			Pending,
		}

		protected BasePromise()
		{
			State = PromiseState.Pending;
		}

		public void ReportFail(Exception ex)
		{
			exception = ex;
			State = PromiseState.Failed;
			if (OnFail != null)
				OnFail(ex);
			Finally();
		}

		public void ReportProgress(float progress)
		{
			if (OnProgress != null)
				OnProgress(progress);
		}

		/// <summary>
		/// Returns false if the Promise has yet to be resolved. If resolved,
		/// sets the state to Fulfilled and returns true.
		/// </summary>
		protected bool Fulfill()
		{
			if (Resolved) return false;

			State = PromiseState.Fulfilled;
			return true;
		}

		public IBasePromise Progress(Action<float> listener)
		{
			OnProgress = AddUnique<float>(OnProgress, listener);
			return this;
		}

		public IBasePromise Fail(Action<Exception> listener)
		{
			if (Failed)
			{
				listener(exception);
				Finally();
			}
			else
				OnFail = AddUnique<Exception>(OnFail, listener);
			return this;
		}

		public IBasePromise Finally(Action listener)
		{
			if (Resolved)
				listener();
			else
				OnFinally = AddUnique(OnFinally, listener);

			return this;
		}

		/// <summary>
		/// Trigger Finally callbacks
		/// </summary>
		protected void Finally()
		{
			if (OnFinally != null)
				OnFinally();
			RemoveAllListeners();
		}

		public void RemoveProgressListeners() { OnProgress = null; }
		public void RemoveFailListeners() { OnFail = null; }
		public virtual void RemoveAllListeners()
		{
			OnProgress = null;
			OnFail = null;
			OnFinally = null;
		}

		/// <summary>
		/// Adds a listener to a callback queue.
		/// </summary>
		/// <returns>The complete list of associated listeners.</returns>
		/// <param name="listeners">Any existing callback queue.</param>
		/// <param name="callback">A callback to add to the queue.</param>
		protected Action AddUnique(Action listeners, Action callback)
		{
			if (listeners == null || !listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}

		/// <summary>
		/// Adds a listener to a callback queue, specifying the Action parameter Type of the listener.
		/// </summary>
		/// <returns>The complete list of associated listeners.</returns>
		/// <param name="listeners">Any existing callback queue.</param>
		/// <param name="callback">A callback to add to the queue.</param>
		protected Action<T> AddUnique<T>(Action<T> listeners, Action<T> callback)
		{
			if (listeners == null || !listeners.GetInvocationList().Contains(callback))
			{
				listeners += callback;
			}
			return listeners;
		}

		protected bool Pending { get { return State == PromiseState.Pending; } }
		protected bool Resolved { get { return State != PromiseState.Pending; } }
		protected bool Fulfilled { get { return State == PromiseState.Fulfilled; } }
		protected bool Failed { get { return State == PromiseState.Failed; } }

		public abstract int ListenerCount();
	}
}
