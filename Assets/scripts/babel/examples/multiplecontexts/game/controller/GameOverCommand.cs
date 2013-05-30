using System;
using UnityEngine;
using babel.examples.multiplecontexts.main;
using babel.extensions.context.api;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.game
{
	public class GameOverCommand : EventCommand
	{
		[Inject]
		public IScore scoreKeeper{get;set;}
		
		[Inject]
		public IGameTimer gameTimer{get;set;}
		
		[Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
		public IEventDispatcher crossContextDispatcher{get;set;}
		
		public override void Execute()
		{
			gameTimer.Stop();
			
			//dispatch between contexts
			Debug.Log("GAME OVER...dispatch across contexts");
			
			crossContextDispatcher.Dispatch(MainEvent.GAME_COMPLETE, scoreKeeper.score);
		}
	}
}

