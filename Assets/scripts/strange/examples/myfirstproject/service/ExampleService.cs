/// Example Service
/// ======================
/// Nothing to see here. Just your typical place to store some data.

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.myfirstproject
{
	public class ExampleService : IExampleService
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		[Inject]
		public IEventDispatcher dispatcher{get;set;}
		
		private string url;
		
		public ExampleService ()
		{
		}

		public void Request(string url)
		{
			this.url = url;
			
			//For now, we'll spoof a web service by running a coroutine for 1 second...
			MonoBehaviour root = contextView.GetComponent<MyFirstProjectRoot>();
			root.StartCoroutine(waitASecond());
		}
		
		private IEnumerator waitASecond()
		{
			yield return new WaitForSeconds(1f);
			
			//...then pass back some fake data
			dispatcher.Dispatch(ExampleEvent.FULFILL_SERVICE_REQUEST, url);
		}
	}
}

