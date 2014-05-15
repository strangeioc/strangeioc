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
 * @class strange.extensions.signal.impl.Signal
 * 
 * This is actually a series of classes defining the Base concrete form for all Signals.
 * 
 * Signals are a type-safe approach to communication that essentially replace the
 * standard EventDispatcher model. Signals can be injected/mapped just like any other
 * object -- as Singletons, as instances, or as values. Signals can even be mapped
 * across Contexts to provide an effective and type-safe way of communicating
 * between the parts of your application.
 * 
 * Signals in Strange use the Action Class as the underlying mechanism for type safety.
 * Unity's C# implementation currently allows up to FOUR parameters in an Action, therefore
 * SIGNALS ARE LIMITED TO FOUR PARAMETERS. If you require more than four, consider
 * creating a value object to hold additional values.
 * 
 * Examples:

		//BASIC SIGNAL CREATION/DISPATCH
		//Create a new signal
		Signal signalWithNoParameters = new Signal();
		//Add a listener
		signalWithNoParameters.AddListener(callbackWithNoParameters);
		//This would throw a compile-time error
		signalWithNoParameters.AddListener(callbackWithOneParameter);
		//Dispatch
		signalWithNoParameters.Dispatch();
		//Remove the listener
		signalWithNoParameters.RemoveListener(callbackWithNoParameters);

		//SIGNAL WITH PARAMETERS
		//Create a new signal with two parameters
 		Signal<int, string> signal = new Signal<int, string>();
 		//Add a listener
		signal.AddListener(callbackWithParamsIntAndString);
		//Add a listener for the duration of precisely one Dispatch
		signal.AddOnce(anotherCallbackWithParamsIntAndString);
		//These all throw compile-time errors
		signal.AddListener(callbackWithParamsStringAndInt);
		signal.AddListener(callbackWithOneParameter);
		signal.AddListener(callbackWithNoParameters);
		//Dispatch
		signal.Dispatch(42, "zaphod");
		//Remove the first listener. The listener added by AddOnce has been automatically removed.
		signal.RemoveListener(callbackWithParamsIntAndString);
 * 
 * @see strange.extensions.signal.api.IBaseSignal
 * @see strange.extensions.signal.impl.BasrSignal
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace strange.extensions.signal.impl
{
    /// Base concrete form for a Signal with no parameters
    public class Signal : BaseSignal
    {
        public event Action Listener = delegate { };
        public event Action OnceListener = delegate { };

        public void AddListener(Action callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action callback)
        {
            OnceListener = this.AddUnique(Listener, callback);
        }
        public void RemoveListener(Action callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            return new List<Type>();
        }
        public void Dispatch()
        {
            Listener();
            OnceListener();
            OnceListener = delegate { };
            base.Dispatch(null);
        }

        private Action AddUnique(Action listeners, Action callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    /// Base concrete form for a Signal with one parameter
    public class Signal<T> : BaseSignal
    {
        public event Action<T> Listener = delegate { };
        public event Action<T> OnceListener = delegate { };

        public void AddListener(Action<T> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T> callback)
        {
            OnceListener = this.AddUnique(Listener, callback);
        }

        public void RemoveListener(Action<T> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            return retv;
        }
        public void Dispatch(T type1)
        {
            Listener(type1);
            OnceListener(type1);
            OnceListener = delegate { };
            object[] outv = { type1 };
            base.Dispatch(outv);
        }

        private Action<T> AddUnique(Action<T> listeners, Action<T> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    /// Base concrete form for a Signal with two parameters
    public class Signal<T, U> : BaseSignal
    {
        public event Action<T, U> Listener = delegate { };
        public event Action<T, U> OnceListener = delegate { };

        public void AddListener(Action<T, U> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T, U> callback)
        {
            OnceListener = this.AddUnique(Listener, callback);
        }

        public void RemoveListener(Action<T, U> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            return retv;
        }
        public void Dispatch(T type1, U type2)
        {
            Listener(type1, type2);
            OnceListener(type1, type2);
            OnceListener = delegate { };
            object[] outv = { type1, type2 };
            base.Dispatch(outv);
        }
        private Action<T, U> AddUnique(Action<T, U> listeners, Action<T, U> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    /// Base concrete form for a Signal with three parameters
    public class Signal<T, U, V> : BaseSignal
    {
        public event Action<T, U, V> Listener = delegate { };
        public event Action<T, U, V> OnceListener = delegate { };

        public void AddListener(Action<T, U, V> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T, U, V> callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action<T, U, V> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            retv.Add(typeof(V));
            return retv;
        }
        public void Dispatch(T type1, U type2, V type3)
        {
            Listener(type1, type2, type3);
            OnceListener(type1, type2, type3);
            OnceListener = delegate { };
            object[] outv = { type1, type2, type3 };
            base.Dispatch(outv);
        }
        private Action<T, U, V> AddUnique(Action<T, U, V> listeners, Action<T, U, V> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

    /// Base concrete form for a Signal with four parameters
    public class Signal<T, U, V, W> : BaseSignal
    {
        public event Action<T, U, V, W> Listener = delegate { };
        public event Action<T, U, V, W> OnceListener = delegate { };

        public void AddListener(Action<T, U, V, W> callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action<T, U, V, W> callback)
        {
            OnceListener = this.AddUnique(OnceListener, callback);
        }

        public void RemoveListener(Action<T, U, V, W> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            retv.Add(typeof(V));
            retv.Add(typeof(W));
            return retv;
        }
        public void Dispatch(T type1, U type2, V type3, W type4)
        {
            Listener(type1, type2, type3, type4);
            OnceListener(type1, type2, type3, type4);
            OnceListener = delegate { };
            object[] outv = { type1, type2, type3, type4 };
            base.Dispatch(outv);
        }

        private Action<T, U, V, W> AddUnique(Action<T, U, V, W> listeners, Action<T, U, V, W> callback)
        {
            if (!listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }
            return listeners;
        }
    }

}
