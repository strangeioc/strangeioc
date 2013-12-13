/// An Asynchronous Command
/// ============================
/// This demonstrates how to use a Command to perform an asynchronous action;
/// for example, if you need to call a web service. The two most important lines
/// are the Retain() and Release() calls.

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.myfirstproject
{
	public class CallWebServiceCommand : EventCommand
	{
		
		[Inject]
		public IExampleModel model{get;set;}
		
		[Inject]
		public IExampleService service{get;set;}
		
		static int counter = 0;
		
		public CallWebServiceCommand()
		{
			++counter;	//This counter is here to demonstrate that a new Command is created each time.
		}
		
		public override void Execute()
		{
			//Retain marks the Command as requiring time to execute.
			//If you call Retain, you MUST have corresponding Release()
			//calls, or you will get memory leaks.
			Retain ();
			
			//Call the service. Listen for a response
			service.dispatcher.AddListener(ExampleEvent.FULFILL_SERVICE_REQUEST, onComplete);
			service.Request("http://www.thirdmotion.com/ ::: " + counter.ToString());
		}
		
		//The payload is in the form of a IEvent
		private void onComplete(IEvent evt)
		{
			//Remember to clean up. Remove the listener.
			service.dispatcher.RemoveListener(ExampleEvent.FULFILL_SERVICE_REQUEST, onComplete);
			
			model.data = evt.data as string;
			dispatcher.Dispatch(ExampleEvent.SCORE_CHANGE, evt.data);
			
			//Remember to call release when done.
			Release ();
		}
	}
}

