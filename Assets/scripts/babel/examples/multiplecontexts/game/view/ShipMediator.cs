/// Ship mediator
/// =====================
/// Make your Mediator as thin as possible. Its function is to mediate
/// between view and app. Don't load it up with behavior that belongs in
/// the View (listening to/controlling interface), Commands (business logic),
/// Models (maintaining state) or Services (reaching out for data).

using System;
using UnityEngine;
using babel.extensions.dispatcher.eventdispatcher.impl;
using babel.extensions.mediation.impl;

namespace babel.examples.multiplecontexts.game
{
	public class ShipMediator : MediatorWithDispatcher
	{
		private ShipView view;
		
		public override void onRegister()
		{
			view = abstractView as ShipView;
			
			//Listen to the view for an event
			view.dispatcher.addListener(ShipView.CLICK_EVENT, onViewClicked);
			
			//Listen to the global event bus for events
			dispatcher.addListener(ExampleEvent.GAME_UPDATE, onGameUpdate);
			
			view.init ();
		}
		
		public override void onRemove()
		{
			//Clean up listeners when the view is about to be destroyed
			view.dispatcher.removeListener(ShipView.CLICK_EVENT, onViewClicked);
			dispatcher.removeListener(ExampleEvent.SCORE_CHANGE, onGameUpdate);
			Debug.Log("Mediator onRemove");
		}
		
		private void onViewClicked()
		{
			Debug.Log("View click detected");
			dispatcher.Dispatch(ExampleEvent.REQUEST_WEB_SERVICE);
		}
		
		private void onGameUpdate(object data)
		{
			//TmEvent evt = data as TmEvent;
			//float score = (float)evt.data;
			//string score = (string)evt.data;
			view.updatePosition();
		}
	}
}

