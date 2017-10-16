/// An example view
/// ==========================
/// 

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace strange.examples.signals
{
	public class ExampleView : View
	{
		public Signal clickSignal = new Signal();
		
		GameObject latestGO;
		
		private float theta = 20f;
		private Vector3 basePosition;
		
		//Publicly settable from Unity3D
		public float edx_WobbleSize = 1f;
		public float edx_WobbleDampen = .9f;
		public float edx_WobbleMin = .001f;
		
		internal void init()
		{
			latestGO = Instantiate(Resources.Load("Textfield")) as GameObject;
			GameObject go = latestGO;
			go.name = "first";

			Renderer renderer = go.GetComponent<Renderer>();
			BoxCollider boxCollider = go.GetComponent<BoxCollider>();

			TextMesh textMesh = go.GetComponent<TextMesh>();
			textMesh.text = "http://www.thirdmotion.com";
			textMesh.font.material.color = Color.red;
			
			Vector3 localPosition = go.transform.localPosition;
			localPosition.x -= renderer.bounds.extents.x;
			localPosition.y += renderer.bounds.extents.y;
			go.transform.localPosition = localPosition;
			
			Vector3 extents = Vector3.zero;
			extents.x = renderer.bounds.size.x;
			extents.y = renderer.bounds.size.y;
			extents.z = renderer.bounds.size.z;
			boxCollider.size = extents;
			boxCollider.center = -localPosition;
			
			go.transform.parent = gameObject.transform;
			
			go.AddComponent<ClickDetector>();
			ClickDetector clicker = go.GetComponent<ClickDetector>() as ClickDetector;
			clicker.clickSignal.AddListener(onClick);
		}
		
		internal void updateScore(string score)
		{
			latestGO = Instantiate(Resources.Load("Textfield")) as GameObject;
			GameObject go = latestGO;
			TextMesh textMesh = go.GetComponent<TextMesh>();
			textMesh.font.material.color = Color.white;
			go.transform.parent = transform;

			textMesh.text = score.ToString();
		}
		
		internal string currentText
		{
			get
			{
				GameObject go = latestGO;
				TextMesh textMesh = go.GetComponent<TextMesh>();
				return textMesh.text;
			}
		}
		
		void Update()
		{
			transform.Rotate(Vector3.up * Time.deltaTime * theta, Space.Self);
		}
		
		void onClick()
		{
			clickSignal.Dispatch();
			startWobble();
		}
		
		private void startWobble()
		{
			StartCoroutine(wobble (edx_WobbleSize));
			basePosition = Vector3.zero;
		}
		
		private IEnumerator wobble(float size)
		{
			while(size > edx_WobbleMin)
			{
				size *= edx_WobbleDampen;
				Vector3 newPosition = basePosition;
				newPosition.x += UnityEngine.Random.Range(-size, size);
				newPosition.y += UnityEngine.Random.Range(-size, size);
				newPosition.z += UnityEngine.Random.Range(-size, size);
				gameObject.transform.localPosition = newPosition;
				yield return null;
			}
			gameObject.transform.localPosition = basePosition;
		}
	}
}

