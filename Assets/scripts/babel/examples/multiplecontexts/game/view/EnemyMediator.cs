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
	public class EnemyMediator : MediatorWithDispatcher
	{
		private EnemyView view;
		
		public override void onRegister()
		{
			view = abstractView as EnemyView;
			updateListeners(true);
			view.init ();
		}
		
		public override void onRemove()
		{
			updateListeners(false);
		}
		
		private void updateListeners(bool value)
		{
			view.dispatcher.updateListener(value, EnemyView.CLICK_EVENT, onViewClicked);
			dispatcher.updateListener( value, GameEvent.GAME_UPDATE, onGameUpdate);
			dispatcher.updateListener( value, GameEvent.GAME_OVER, onGameOver);
			
			dispatcher.addListener(GameEvent.RESTART_GAME, onRestart);
		}
		
		private void onViewClicked()
		{
			dispatcher.Dispatch(GameEvent.ADD_TO_SCORE, 10);
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

