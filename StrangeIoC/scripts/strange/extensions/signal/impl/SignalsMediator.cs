using strange.extensions.mediation.impl;
using strange.extensions.signal.api;
using strange.extensions.signal.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.signal.impl
{
    public class SignalsMediator : Mediator
    {
        // Using lazy instantiation so not to use unneccessary memory
        private List<ISignalBinding> _signalBindings;
        protected List<ISignalBinding> SignalBindings 
        {
            get
            {
                if (_signalBindings == null) _signalBindings = new List<ISignalBinding>();
                return _signalBindings;
            }
        }

        override public void OnPreRemove()
        {
            // Perhaps no signals were added, so no need to call the getter which would then instantiate a list
            if (_signalBindings==null) return;

            // Remove each binding
            SignalBindings.ForEach(r => r.RemoveBinding());
        }

        protected void RegisterListener(Signal signal, Action callback)
        {
            SignalBindings.Add(signal.AddListener(callback));
        }

        protected void RegisterListener<T>(Signal<T> signal, Action<T> callback)
        {
            SignalBindings.Add(signal.AddListener(callback));
        }

        protected void RegisterListener<T, U>(Signal<T, U> signal, Action<T, U> callback)
        {
            SignalBindings.Add(signal.AddListener(callback));
        }

        protected void RegisterListener<T, U, V>(Signal<T, U, V> signal, Action<T, U, V> callback)
        {
            SignalBindings.Add(signal.AddListener(callback));
        }

        protected void RegisterListener<T, U, V, W>(Signal<T, U, V, W> signal, Action<T, U, V, W> callback)
        {
            SignalBindings.Add(signal.AddListener(callback));
        }
    }
}
