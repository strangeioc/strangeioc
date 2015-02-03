using System;
using System.Reflection;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using strange.extensions.reflector.api;
using strange.extensions.signal.impl;
using UnityEngine;

namespace strange.extensions.mediation
{
    public class SignalMediationBinder : MediationBinder
    {
        [Inject]
        public IReflectionBinder reflectionBinder { get; set; }

        protected override MonoBehaviour createMediator(MonoBehaviour mono, Type mediatorType)
        {
            MonoBehaviour mediator = base.createMediator(mono, mediatorType);
            if (mediator is IMediator)
            {
                HandleDelegates(mediator, mediatorType, true);
            }
            return mediator;
        }

        private void HandleDelegates(MonoBehaviour mono, Type mediatorType, bool toAdd)
        {
            IReflectedClass reflectedClass = reflectionBinder.Get(mediatorType);

            //Let's GetInstance some Signals and add listeners

            foreach (var pair in reflectedClass.attrMethods)
            {
                if (pair.Value is ListensTo)
                {
                    ListensTo attr = (ListensTo)pair.Value;
                    ISignal signal = (ISignal)injectionBinder.GetInstance(attr.type);
                    if (toAdd)
                        AssignDelegate(mono, signal, pair.Key);
                    else
                        RemoveDelegate(mono, signal, pair.Key);
                }
            }
        }

        private void RemoveDelegate(MonoBehaviour mediator, ISignal signal, MethodInfo method)
        {
            if (signal.GetType().BaseType.IsGenericType) //e.g. Signal<T>, Signal<T,U> etc.
            {
                Delegate toRemove = Delegate.CreateDelegate(signal.listener.GetType(), mediator, method);
                signal.listener = Delegate.Remove(signal.listener,toRemove);
            }
            else
            {
                ((Signal)signal).RemoveListener((Action)Delegate.CreateDelegate(typeof(Action), mediator, method)); //Assign and cast explicitly for Type == Signal case
            }

        }

        private void AssignDelegate(MonoBehaviour mediator, ISignal signal, MethodInfo method)
        {
            if (signal.GetType().BaseType.IsGenericType)
            {
                signal.listener = Delegate.CreateDelegate(signal.listener.GetType(), mediator, method); //e.g. Signal<T>, Signal<T,U> etc.
            }
            else
            {
                ((Signal)signal).AddListener((Action)Delegate.CreateDelegate(typeof(Action), mediator, method)); //Assign and cast explicitly for Type == Signal case
            }

        }



        protected override void removeMediator(IMediator mediator)
        {
            //Unbind signals to methods
            HandleDelegates((MonoBehaviour)mediator, mediator.GetType(), false);

            base.removeMediator(mediator);
        }
    }
}
