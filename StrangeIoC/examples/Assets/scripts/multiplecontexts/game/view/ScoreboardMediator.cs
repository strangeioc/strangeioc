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

using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ScoreboardMediator : EventMediator
	{
		[Inject]
		public ScoreboardView view{ get; set;}

		[Inject]
		public IScore model{ get; set;}
		
		private const string SCORE_STRING = "score: ";
		private const string LIVES_STRING = "lives remaining: ";
		
		public override void OnRegister()
		{
			UpdateListeners(true);
			view.init (SCORE_STRING + "0", LIVES_STRING + model.lives.ToString());
		}
		
		public override void OnRemove()
		{
			UpdateListeners(false);
		}
		
		private void UpdateListeners(bool value)
		{
			dispatcher.UpdateListener(value, GameEvent.SCORE_CHANGE, onScoreChange);
			dispatcher.UpdateListener(value, GameEvent.LIVES_CHANGE, onLivesChange);
			dispatcher.UpdateListener(value, GameEvent.GAME_OVER, onGameOver);
			
			view.dispatcher.AddListener(ScoreboardView.REPLAY, onReplay);
			view.dispatcher.AddListener(ScoreboardView.REMOVE_CONTEXT, onRemoveContext);
			dispatcher.AddListener(GameEvent.RESTART_GAME, onRestart);
		}
		
		private void onScoreChange(IEvent evt)
		{
			string score = SCORE_STRING + (int)evt.data;
			view.updateScore(score);
		}
		
		private void onLivesChange(IEvent evt)
		{
			string lives = LIVES_STRING + (int)evt.data;
			view.updateLives(lives);
		}
		
		private void onGameOver()
		{
			UpdateListeners(false);
			view.gameOver();
		}
		
		private void onReplay()
		{
			dispatcher.Dispatch(GameEvent.REPLAY);
		}
		
		private void onRemoveContext()
		{
			dispatcher.Dispatch(GameEvent.REMOVE_SOCIAL_CONTEXT);
		}
		
		private void onRestart()
		{
			OnRegister();
		}
	}
}

