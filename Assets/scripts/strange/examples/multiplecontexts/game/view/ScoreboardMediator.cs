using System;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ScoreboardMediator : EventMediator
	{
		private ScoreboardView view;
		
		private const string SCORE_STRING = "score: ";
		private const string LIVES_STRING = "lives remaining: ";
		
		public override void onRegister()
		{
			view = abstractView as ScoreboardView;
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

