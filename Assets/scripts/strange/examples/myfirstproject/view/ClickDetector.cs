/// Just a simple MonoBehaviour Click Detector

using System;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace strange.examples.myfirstproject
{
	public class ClickDetector : EventView
	{
		public const string CLICK = "CLICK";
		
		void OnMouseDown()
		{
			dispatcher.Dispatch(CLICK);
		}
	}
}

