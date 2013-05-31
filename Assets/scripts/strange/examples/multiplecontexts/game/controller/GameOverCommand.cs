using System;
using UnityEngine;
using strange.examples.multiplecontexts.main;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.game
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

