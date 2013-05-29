using System;
using System.Collections;
using UnityEngine;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.mediation.impl;

namespace babel.examples.multiplecontexts.game
{
	public class EnemyView : ViewWithDispatcher
	{
		internal const string CLICK_EVENT = "CLICK_EVENT";
		
		private float theta = 0f;
		private Vector3 basePosition;
		
		//Publicly settable from Unity3D
		public float edx_WobbleForce = .4f;
		public float edx_WobbleIncrement = .1f;
		
		internal void init()
		{
			gameObject.AddComponent<ClickDetector>();
			ClickDetector clicker = gameObject.GetComponent<ClickDetector>();
			clicker.dispatcher.addListener(ClickDetector.CLICK, onClick);
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
			gameObject.transform.RotateAround(Vector3.forward, edx_WobbleForce * Mathf.Sin(theta));
		}
	}
}

