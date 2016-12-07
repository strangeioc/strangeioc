using strange.extensions.mediation.api;
using strange.extensions.signal.impl;
using UnityEngine;

namespace strange.unittests
{
	public class SignalMediator : IMediator
	{
		public GameObject contextView { get; set; }

		public static int Value = 0;

		[ListensTo(typeof (OneArgSignal))]
		public void OneArgMethod(int myArg)
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

	public class NoArgSignal : Signal { }
	public class OneArgSignal : Signal<int> { }
	public class TwoArgSignal : Signal<int, bool> { }

}
