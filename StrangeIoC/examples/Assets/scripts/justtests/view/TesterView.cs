using System;
using strange.extensions.mediation.impl;

namespace strange.examples.justtests.view
{
	public class TesterView : View, ITesterView
	{
		public void Init()
		{
			UnityEngine.Debug.LogWarning ("Inited");
		}
	}
}

