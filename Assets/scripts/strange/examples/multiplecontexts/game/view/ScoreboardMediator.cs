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
		
		private const string SCORE_STRING = "score: ";
		private const string LIVES_STRING = "lives remaining: ";
		
		public override void onRegister()
		{
			updateListeners(true);
			//I'm cheating a little here for the sake of simplicity.
			//The number 3 should be supplied from an injected config.
			view.init (SCORE_STRING + "0", LIVES_STRING + "3");
		}
		
		public override void onRemove()
		{
			updateListeners(false);
		}
		
		private void updateListeners(bool value)
		{
			dispatcher.updateListener(value, GameEvent.SCORE_CHANGE, onScoreChange);
			dispatcher.updateListener(value, GameEvent.LIVES_CHANGE, onLivesChange);
			dispatcher.updateListener(value, GameEvent.GAME_OVER, onGameOver);
			
			view.dispatcher.addListener(ScoreboardView.REPLAY, onReplay);
			dispatcher.addListener(GameEvent.RESTART_GAME, onRestart);
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
			updateListeners(false);
			view.gameOver();
		}
		
		private void onReplay()
		{
			dispatcher.Dispatch(GameEvent.REPLAY);
		}
		
		private void onRestart()
		{
			onRegister();
		}
	}
}

