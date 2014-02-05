/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ShipView : EventView
	{
		internal const string CLICK_EVENT = "CLICK_EVENT";
		
		private float theta = 0f;
		private Vector3 basePosition;
		
		//Publicly settable from Unity3D
		public float edx_WobbleSize = .1f;
		public float edx_WobbleIncrement = .1f;
		
		private ClickDetector clicker;

		internal void init()
		{
			gameObject.AddComponent<ClickDetector>();
			clicker = gameObject.GetComponent<ClickDetector>();
			StartCoroutine (addClicker ());
		}

		private IEnumerator addClicker()
		{
			yield return null;
			clicker.dispatcher.AddListener(ClickDetector.CLICK, onClick);
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
			gameObject.transform.Rotate(Vector3.up, edx_WobbleSize * Mathf.Sin(theta));
		}
	}
}

