/// Ship mediator
/// =====================
/// Make your Mediator as thin as possible. Its function is to mediate
/// between view and app. Don't load it up with behavior that belongs in
/// the View (listening to/controlling interface), Commands (business logic),
/// Models (maintaining state) or Services (reaching out for data).

using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ShipMediator : MediatorWithDispatcher
	{
		private ShipView view;
		
		public override void onRegister()
		{
			view = abstractView as ShipView;
			updateListeners(true);
			view.init ();
		}
		
		public override void onRemove()
		{
			updateListeners(false);
		}
		
		private void updateListeners(bool value)
		{
			view.dispatcher.updateListener(value, ShipView.CLICK_EVENT, onViewClicked);
			dispatcher.updateListener( value, GameEvent.GAME_UPDATE, onGameUpdate);
			dispatcher.updateListener( value, GameEvent.GAME_OVER, onGameOver);
			
			dispatcher.addListener(GameEvent.RESTART_GAME, onRestart);
		}
		
		private void onViewClicked()
		{
			dispatcher.Dispatch(GameEvent.SHIP_DESTROYED);
		}
		
		private void onGameUpdate(object data)
		{
			view.updatePosition();
		}
		
		private void onGameOver()
		{
			updateListeners(false);
		}
		
		private void onRestart()
		{
			onRegister();
		}
	}
}

