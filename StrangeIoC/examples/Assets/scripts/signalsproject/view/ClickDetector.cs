/// Uses a signal instead of an EventDispatcher

using System;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace strange.examples.signals
{
	public class ClickDetector : View
	{
		// Note how we're using a signal now
		public Signal clickSignal = new Signal();
		
		void OnMouseDown()
		{
			clickSignal.Dispatch();
		}
	}
}

