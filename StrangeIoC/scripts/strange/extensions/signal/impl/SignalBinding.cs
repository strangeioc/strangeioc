using strange.extensions.signal.api;
using strange.extensions.signal.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.signal.impl
{
    public class SignalBinding : ISignalBinding
    {
        public Signal Signal { get; private set; }
        public Action Callback { get; private set; }

        public SignalBinding(Signal signal, Action callback)
        {
            Signal = signal;
            Callback = callback;
        }

        public void RemoveBinding()
        {
            Signal.RemoveListener(Callback);
        }
    }

    public class SignalBinding<T> : ISignalBinding
    {
        public Signal<T> Signal { get; private set; }
        public Action<T> Callback { get; private set; }

        public SignalBinding(Signal<T> signal, Action<T> callback)
        {
            Signal = signal;
            Callback = callback;
        }

        public void RemoveBinding()
        {
            Signal.RemoveListener(Callback);
        }
    }

    public class SignalBinding<T,U> : ISignalBinding
    {
        public Signal<T,U> Signal { get; private set; }
        public Action<T,U> Callback { get; private set; }

        public SignalBinding(Signal<T,U> signal, Action<T,U> callback)
        {
            Signal = signal;
            Callback = callback;
        }

        public void RemoveBinding()
        {
            Signal.RemoveListener(Callback);
        }
    }

    public class SignalBinding<T, U, V> : ISignalBinding
    {
        public Signal<T, U, V> Signal { get; private set; }
        public Action<T, U, V> Callback { get; private set; }

        public SignalBinding(Signal<T, U, V> signal, Action<T, U, V> callback)
        {
            Signal = signal;
            Callback = callback;
        }

        public void RemoveBinding()
        {
            Signal.RemoveListener(Callback);
        }
    }

    public class SignalBinding<T, U, V, W> : ISignalBinding
    {
        public Signal<T, U, V, W> Signal { get; private set; }
        public Action<T, U, V, W> Callback { get; private set; }

        public SignalBinding(Signal<T, U, V, W> signal, Action<T, U, V, W> callback)
        {
            Signal = signal;
            Callback = callback;
        }

        public void RemoveBinding()
        {
            Signal.RemoveListener(Callback);
        }
    }
}
