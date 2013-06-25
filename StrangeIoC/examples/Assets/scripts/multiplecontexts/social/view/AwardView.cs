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

/// View for text at end of game
/// ==========================
/// Holds some text

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.social
{
	public class AwardView : View
	{
		private Vector3 basePosition;
		private TextMesh textfield;
		
		internal void init()
		{
			GameObject go = Instantiate(Resources.Load("AwardText")) as GameObject;
			go.transform.parent = gameObject.transform;
			textfield = go.GetComponent<TextMesh>();
		}
		
		internal void setTest(string message)
		{
			textfield.text = message;
		}
		
		void Update()
		{
			Vector3 dest = Vector3.zero;
			Vector3 scale = transform.localScale;
			scale += (dest - scale) * .009f;
			transform.localScale = scale;
		}
	}
}

