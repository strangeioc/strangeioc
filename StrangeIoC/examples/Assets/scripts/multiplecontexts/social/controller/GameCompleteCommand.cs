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

/// GameCompleteCommand
/// ============================
/// This Command captures the GameComplete event from Game and starts some social stuff going

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.multiplecontexts.social
{
	public class GameCompleteCommand : EventCommand
	{
		
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		[Inject]
		public ISocialService social{get;set;}
		
		//Remember back in StartCommand when I said we'd need the userVO again?
		[Inject]
		public UserVO userVO{get;set;}
		
		public override void Execute()
		{
			Retain ();
			int score = (int)evt.data;
			
			//Set the current score
			userVO.currentScore = score;
			
			Debug.Log ("Social SCENE KNOWS THAT GAME IS OVER. Your score is: " + score);
			social.dispatcher.AddListener(SocialEvent.FULFILL_FRIENDS_REQUEST, onResponse);
			social.FetchScoresForFriends();
		}
		
		private void onResponse(IEvent evt)
		{
			social.dispatcher.RemoveListener(SocialEvent.FULFILL_FRIENDS_REQUEST, onResponse);
			ArrayList list = evt.data as ArrayList;
			
			//Save the list as the data for the next item in the sequence
			data = list;
			Release();
		}
	}
}

