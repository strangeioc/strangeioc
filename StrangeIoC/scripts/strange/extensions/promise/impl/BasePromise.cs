using System;
using System.Linq;
using strange.extensions.promise.api;

namespace strange.extensions.signal.impl
{
    public abstract class BasePromise : IPromise
    {
        private event Action<float> OnProgress;
        private event Action<Exception> OnFail;
        private event Action OnFinally;

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

        protected bool Fulfill()
        {
            if (Resolved) return false;

            State = PromiseState.Fulfilled;
            return true;
        }

        public IPromise Progress(Action<float> listener)
        {
            OnProgress = AddUnique<float>(OnProgress, listener);
            return this;
        }

        public IPromise Fail(Action<Exception> listener)
        {
            OnFail = AddUnique<Exception>(OnFail, listener);
            return this;
        }

        public IPromise Finally(Action listener)
        {
            if (Resolved)
                listener();
            else
                OnFinally = AddUnique(OnFinally, listener);

            return this;
        }

        protected void Finally()
        {
            if (OnFinally != null)
                OnFinally();
        }

        public void RemoveProgressListeners() { OnProgress = null; }
        public void RemoveFailListeners() { OnFail = null; }
        public virtual void RemoveAllListeners()
        {
            OnProgress = null;
            OnFail = null;
        }

        protected Action AddUnique(Action listeners, Action callback)
        {
            if (listeners == null || !listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }

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
