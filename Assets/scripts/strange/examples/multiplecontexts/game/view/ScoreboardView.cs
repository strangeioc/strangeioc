using System;
using System.Collections;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ScoreboardView : EventView
	{
		internal const string REPLAY = "REPLAY";
		
		private Vector3 basePosition;
		private string scoreString;
		private string livesString;
		private Rect scoreRect;
		private Rect replayRect;
		private bool gameOn;

		internal void init(string score, string lives)
		{
			scoreString = score;
			scoreRect = new Rect(5f, 5f, 120f, 50f);
			replayRect = new Rect(Screen.width / 2f, Screen.height / 2f, 100f, 100f);
			livesString = lives;
			gameOn = true;
		}
		
		private void OnGUI()
		{
			GUI.TextField(scoreRect, scoreString + "\n" + livesString);
			
			if (!gameOn)
			{
				if (GUI.Button(replayRect, "Replay"))
				{
					dispatcher.Dispatch(REPLAY);
				}
			}
		}
		
		internal void updateScore(string value)
		{
			scoreString = value;
		}
		
		internal void updateLives(string value)
		{
			livesString = value;
		}
		
		internal void gameOver()
		{
			gameOn = false;
		}
	}
}

