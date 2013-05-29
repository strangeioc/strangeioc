using System;
using UnityEngine;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.game
{
	public class GameOverCommand : EventCommand
	{
		[Inject]
		public IScore scoreKeeper{get;set;}
		
		[Inject]
		public IGameTimer gameTimer{get;set;}
		
		public override void Execute()
		{
			gameTimer.Stop();
			
			//dispatch between contexts
			Debug.Log("GAME OVER...dispatch across contexts");
		}
	}
}

