/// GameCompleteCommand
/// ============================
/// This Command captures the GameComplete event from Game and starts some social stuff going

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.sequencer.api;
using strange.extensions.sequencer.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.social
{
	public class GameCompleteCommand : SequenceCommand
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
			TmEvent evt = data as TmEvent;
			int score = (int)evt.data;
			
			//Set the current score
			userVO.currentScore = score;
			
			Debug.Log ("Social SCENE KNOWS THAT GAME IS OVER. Your score is: " + score);
			social.dispatcher.addListener(SocialEvent.FULFILL_FRIENDS_REQUEST, onResponse);
			social.FetchScoresForFriends();
		}
		
		private void onResponse(object payload)
		{
			social.dispatcher.removeListener(SocialEvent.FULFILL_FRIENDS_REQUEST, onResponse);
			TmEvent evt = payload as TmEvent;
			ArrayList list = evt.data as ArrayList;
			
			//Save the list as the data for the next item in the sequence
			data = list;
			Release();
		}
	}
}

