using System;
using System.Linq;

namespace strange.extensions.signal.impl
{
	public class Promise : BasePromise
	{
		protected event Action Listener = null;
		public void Dispatch()
		{
			if (Fulfill())
				CallListener();
			Finally();
		}

		public Promise Then(Action action)
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

	public class Promise<T> : BasePromise
	{
		private T t;

		protected event Action<T> Listener = null;
		public void Dispatch(T t)
		{
			if (Fulfill())
			{
				this.t = t;
				CallListener();
				Finally();
			}
		}

		public Promise<T> Then(Action<T> action)
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

	public class Promise<T,U> : BasePromise
	{
		private T t;
		private U u;

		protected event Action<T,U> Listener = null;
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

		public Promise<T,U> Then(Action<T,U> action)
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

	public class Promise<T,U,V> : BasePromise
	{
		private T t;
		private U u;
		private V v;

		protected event Action<T, U, V> Listener = null;
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

		public Promise<T,U,V> Then(Action<T, U, V> action)
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

	public class Promise<T,U,V,W> : BasePromise
	{
		private T t;
		private U u;
		private V v;
		private W w;

		protected event Action<T, U, V, W> Listener = null;
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

		public Promise<T, U, V,W> Then(Action<T, U, V, W> action)
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
