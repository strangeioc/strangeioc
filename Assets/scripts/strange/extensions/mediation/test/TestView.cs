using System;
using UnityEngine;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;

namespace strange.extensions.mediation.test
{
	public class TestView : RuntimeTester
	{
		View view;
		
		public TestView ()
		{
		}
		
		void Awake()
		{
			view = gameObject.GetComponent<View>();
			failIf(view == null, "TestView requires a View");
			Debug.Log("Awake");
		}
		
		void Update()
		{
			if (view != null)
			{
				GameObject.DestroyImmediate(view);
				failIf(view != null, "TestView was supposed to be destroyed");
			}
			
		}
	}
}

