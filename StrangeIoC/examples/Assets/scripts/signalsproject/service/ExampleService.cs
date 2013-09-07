/// The service, now with signals

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.signals
{
	public class ExampleService : IExampleService
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		//The interface demands this signal
		[Inject]
		public FulfillWebServiceRequestSignal fulfillSignal{get;set;}
		
		private string url;
		
		public ExampleService ()
		{
		}

		public void Request(string url)
		{
			this.url = url;
			
			MonoBehaviour root = contextView.GetComponent<SignalsRoot>();
			root.StartCoroutine(waitASecond());
		}
		
		private IEnumerator waitASecond()
		{
			yield return new WaitForSeconds(1f);
			
			//Pass back some fake data via a Signal
			fulfillSignal.Dispatch(url);
		}
	}
}

