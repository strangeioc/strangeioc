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

