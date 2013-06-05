/// Example mediator
/// =====================
/// Make your Mediator as thin as possible. Its function is to mediate
/// between view and app. Don't load it up with behavior that belongs in
/// the View (listening to/controlling interface), Commands (business logic),
/// Models (maintaining state) or Services (reaching out for data).

using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.myfirstproject
{
	public class ExampleMediator : EventMediator
	{
		//This is how your Mediator knows about your View.
		[Inject]
		public ExampleView view{ get; set;}
		
		public override void onRegister()
		{
			
			//Listen to the view for an event
			view.dispatcher.addListener(ExampleView.CLICK_EVENT, onViewClicked);
			
			//Listen to the global event bus for events
			dispatcher.addListener(ExampleEvent.SCORE_CHANGE, onScoreChange);
			
			view.init ();
		}
		
		public override void onRemove()
		{
			//Clean up listeners when the view is about to be destroyed
			view.dispatcher.removeListener(ExampleView.CLICK_EVENT, onViewClicked);
			dispatcher.removeListener(ExampleEvent.SCORE_CHANGE, onScoreChange);
			Debug.Log("Mediator onRemove");
		}
		
		private void onViewClicked()
		{
			Debug.Log("View click detected");
			dispatcher.Dispatch(ExampleEvent.REQUEST_WEB_SERVICE);
		}
		
		private void onScoreChange(IEvent evt)
		{
			//float score = (float)evt.data;
			string score = (string)evt.data;
			view.updateScore(score);
		}
	}
}

