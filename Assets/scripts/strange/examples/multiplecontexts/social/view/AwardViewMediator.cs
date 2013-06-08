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
using  strange.examples.multiplecontexts.game;

namespace strange.examples.multiplecontexts.social
{
	public class AwardViewMediator : EventMediator
	{
		[Inject]
		public AwardView view{ get; set;}
		
		public override void OnRegister()
		{
			//Listen to the global event bus for events
			dispatcher.AddListener(GameEvent.RESTART_GAME, onGameRestart);
			dispatcher.AddListener(SocialEvent.REWARD_TEXT, onReward);
			
			view.init ();
		}
		
		public override void OnRemove()
		{
			//Clean up listeners when the view is about to be destroyed
			dispatcher.RemoveListener(GameEvent.RESTART_GAME, onGameRestart);
			dispatcher.RemoveListener(SocialEvent.REWARD_TEXT, onReward);
		}
		
		private void onGameRestart()
		{
			GameObject.Destroy(gameObject);
		}
		
		private void onReward(IEvent evt)
		{
			view.setTest(evt.data as string);
		}
	}
}

