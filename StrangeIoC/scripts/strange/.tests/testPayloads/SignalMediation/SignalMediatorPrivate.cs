using strange.extensions.mediation.api;
using UnityEngine;

namespace strange.unittests
{
    public class SignalMediatorPrivate : IMediator
    {
            public GameObject contextView { get; set; }

            public static int Value = 0;

            [ListensTo(typeof(OneArgSignal))]
            private void OneArgMethod(int myArg)
            {
                Value += myArg;
            }

            public void PreRegister()
            {
            }

            public void OnRegister()
            {
            }

            public void OnRemove()
            {
            }

            public void OnEnabled()
            {
            }

            public void OnDisabled()
            {
            }
    }
}
