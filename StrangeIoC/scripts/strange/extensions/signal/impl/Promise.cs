using System;

namespace strange.extensions.signal.impl
{
    public class BasePromise
    {
        public BasePromise()
        {
            Dispatched = false;
        }

        public bool Dispatched { get; protected set; }
    }

    public class Promise : BasePromise
    {
        protected event Action Listener = delegate { };
        public void Dispatch()
        {
            if (Dispatched) return;
            Dispatched = true;

            Listener();
        }

        public Promise Then(Action action)
        {
            if (Dispatched)
                action();
            else
                Listener += action;

	        return this;
        }
		
        public void RemoveListener(Action action) { Listener -= action; }
        public void RemoveAllListeners() { Listener = delegate { }; }
    }

    public class Promise<T> : BasePromise
    {
        private T t;

        protected event Action<T> Listener = delegate { };
        public void Dispatch(T t)
        {
            if (Dispatched) return;
            Dispatched = true;

            this.t = t;
            Listener(t);
        }

        public Promise<T> Then(Action<T> action)
        {
            if (Dispatched)
                action(this.t);
            else
                Listener += action;
	        return this;
        }

        public void RemoveListener(Action<T> action) { Listener -= action; }
        public void RemoveAllListeners() { Listener = delegate { }; }
    }

    public class Promise<T,U> : BasePromise
    {
        private T t;
        private U u;

        protected event Action<T,U> Listener = delegate { };
        public void Dispatch(T t, U u)
        {
            if (Dispatched) return;
            Dispatched = true;

            this.t = t;
            this.u = u;
            Listener(t,u);
        }

        public Promise<T,U> Then(Action<T,U> action)
        {
            if (Dispatched)
                action(this.t, this.u);
            else
                Listener += action;
	        return this;
        }

        public void RemoveListener(Action<T,U> action) { Listener -= action; }
        public void RemoveAllListeners() { Listener = delegate { }; }
    }

    public class Promise<T,U,V> : BasePromise
    {
        private T t;
        private U u;
        private V v;

        protected event Action<T, U, V> Listener = delegate { };
        public void Dispatch(T t, U u, V v)
        {
            if (Dispatched) return;
            Dispatched = true;

            this.t = t;
            this.u = u;
            this.v = v;
            Listener(t, u, v);
        }

		public Promise<T,U,V> Then(Action<T, U, V> action)
        {
            if (Dispatched)
                action(this.t, this.u, this.v);
            else
                Listener += action;
			return this;
        }

        public void RemoveListener(Action<T, U, V> action) { Listener -= action; }
        public void RemoveAllListeners() { Listener = delegate { }; }
    }

    public class Promise<T,U,V,W> : BasePromise
    {
        private T t;
        private U u;
        private V v;
        private W w;

        protected event Action<T, U, V, W> Listener = delegate { };
        public void Dispatch(T t, U u, V v, W w)
        {
            if (Dispatched) return;
            Dispatched = true;

            this.t = t;
            this.u = u;
            this.v = v;
            this.w = w;
            Listener(t, u, v, w);
        }

		public Promise<T, U, V,W> Then(Action<T, U, V, W> action)
        {
            if (Dispatched)
                action(t, u, v, w);
            else
                Listener += action;
			return this;
        }

        public void RemoveListener(Action<T, U, V, W> action) { Listener -= action; }
        public void RemoveAllListeners() { Listener = delegate { }; }
    }
}
