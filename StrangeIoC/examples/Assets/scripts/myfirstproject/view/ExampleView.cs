/// An example view
/// ==========================
/// The view is where you program and configure the particulars of an item
/// in a scene. For example, if you have a GameObject with buttons and a
/// test readout, wire all that into this class.
/// 
/// By default, Views do not have access to the common Event bus. While you
/// could inject it, we STRONGLY recommend against doing this. Views are by
/// nature volatile, possibly the piece of your app most likely to change.
/// Mediation mapping allows you to automatically attach a 'Mediator' class
/// whose responsibility it is to connect the View to the rest of the app.
/// 
/// Building a view in code here. Ordinarily, you'd do this in the scene.
/// You could argue that this code is kind of messy...not ideal for a demo...
/// but that's kind of the point. View code is often highly volatile and
/// reactive. It gets messy. Let your view be what it needs to be while
/// insulating the rest of your app from this chaos.

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.myfirstproject
{
	public class ExampleView : View
	{
		internal const string CLICK_EVENT = "CLICK_EVENT";
		
		[Inject]
		public IEventDispatcher dispatcher{get;set;}
		
		private float theta = 20f;
		private Vector3 basePosition;
		
		//Publicly settable from Unity3D
		public float edx_WobbleSize = 1f;
		public float edx_WobbleDampen = .9f;
		public float edx_WobbleMin = .001f;
		
		internal void init()
		{
			GameObject go = Instantiate(Resources.Load("Textfield")) as GameObject;
			
			TextMesh textMesh = go.GetComponent<TextMesh>();
			textMesh.text = "http://www.thirdmotion.com";
			textMesh.font.material.color = Color.red;
			
			Vector3 localPosition = go.transform.localPosition;
			localPosition.x -= go.GetComponent<Renderer>().bounds.extents.x;
			localPosition.y += go.GetComponent<Renderer>().bounds.extents.y;
			go.transform.localPosition = localPosition;
			
			Vector3 extents = Vector3.zero;
			extents.x = go.GetComponent<Renderer>().bounds.size.x;
			extents.y = go.GetComponent<Renderer>().bounds.size.y;
			extents.z = go.GetComponent<Renderer>().bounds.size.z;
			(go.GetComponent<Collider>() as BoxCollider).size = extents;
			(go.GetComponent<Collider>() as BoxCollider).center = -localPosition;
			
			go.transform.parent = gameObject.transform;
			
			go.AddComponent<ClickDetector>();
			ClickDetector clicker = go.GetComponent<ClickDetector>() as ClickDetector;
			clicker.dispatcher.AddListener(ClickDetector.CLICK, onClick);
		}
		
		internal void updateScore(string score)
		{
			GameObject go = Instantiate(Resources.Load("Textfield")) as GameObject;
			TextMesh textMesh = go.GetComponent<TextMesh>();
			textMesh.font.material.color = Color.white;
			go.transform.parent = transform;

			textMesh.text = score.ToString();
		}
		
		void Update()
		{
			transform.Rotate(Vector3.up * Time.deltaTime * theta, Space.Self);
		}
		
		void onClick()
		{
			dispatcher.Dispatch(CLICK_EVENT);
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

