/// UserTileView's mediator
/// =====================
/// Make your Mediator as thin as possible. Its function is to mediate
/// between view and app. Don't load it up with behavior that belongs in
/// the View (listening to/controlling interface), Commands (business logic),
/// Models (maintaining state) or Services (reaching out for data).

using System;
using System.Collections;
using UnityEngine;
using strange.examples.multiplecontexts.game;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.social
{
	public class UserTileMediator : MediatorWithDispatcher
	{
		private UserTileView view;
		
		[Inject]
		public UserVO userVO{get;set;}
		
		public override void onRegister()
		{
			view = abstractView as UserTileView;

			dispatcher.addListener(GameEvent.RESTART_GAME, onGameRestart);
			
			view.init ();
		}
		
		public override void onRemove()
		{
			//Clean up listeners when the view is about to be destroyed
			dispatcher.removeListener(GameEvent.RESTART_GAME, onGameRestart);
		}
		
		private void onGameRestart()
		{
			UserVO viewUserVO = view.getUser();
			if (viewUserVO != null && viewUserVO.serviceId != userVO.serviceId)
			{
				GameObject.Destroy(gameObject);
			}
		}
	}
}

