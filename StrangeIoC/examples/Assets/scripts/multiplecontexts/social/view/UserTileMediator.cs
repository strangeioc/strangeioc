/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

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
	public class UserTileMediator : EventMediator
	{
		[Inject]
		public UserTileView view{ get; set;}
		
		[Inject]
		public UserVO userVO{get;set;}
		
		public override void OnRegister()
		{
			dispatcher.AddListener(GameEvent.RESTART_GAME, onGameRestart);
			
			view.init ();
		}
		
		public override void OnRemove()
		{
			//Clean up listeners when the view is about to be destroyed
			dispatcher.RemoveListener(GameEvent.RESTART_GAME, onGameRestart);
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

