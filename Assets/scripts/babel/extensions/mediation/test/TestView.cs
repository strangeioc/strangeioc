using System;
using UnityEngine;
using babel.extensions.mediation.api;
using babel.extensions.mediation.impl;

namespace babel.extensions.mediation.test
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

