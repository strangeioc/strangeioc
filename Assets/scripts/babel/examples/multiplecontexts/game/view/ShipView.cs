using System;
using System.Collections;
using UnityEngine;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.mediation.impl;

namespace babel.examples.multiplecontexts.game.view
{
	public class ShipView : View
	{
		internal const string CLICK_EVENT = "CLICK_EVENT";
		
		[Inject]
		public IEventDispatcher dispatcher{get;set;}
		
		private float theta = 0f;
		private Vector3 basePosition;
		
		//Publicly settable from Unity3D
		public float edx_WobbleSize = 1f;
		public float edx_WobbleIncrement = 1f;
		
		internal void init()
		{
			
		}
		
		internal void updatePosition()
		{
			wobble();
		}
		
		void onClick()
		{
			dispatcher.Dispatch(CLICK_EVENT);
		}
		
		void wobble()
		{
			theta += edx_WobbleIncrement;
			gameObject.transform.RotateAroundLocal(Vector3.up, edx_WobbleSize * Mathf.Sin(theta));
			
		}
	}
}

