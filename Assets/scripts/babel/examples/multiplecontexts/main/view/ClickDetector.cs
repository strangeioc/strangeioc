/// Just a simple MonoBehaviour Click Detector

using System;
using UnityEngine;
using babel.extensions.mediation.impl;

namespace babel.examples.multiplecontexts.main.view
{
	public class ClickDetector : ViewWithDispatcher
	{
		public const string CLICK = "CLICK";
		
		void OnMouseDown()
		{
			dispatcher.Dispatch(CLICK);
		}
	}
}

