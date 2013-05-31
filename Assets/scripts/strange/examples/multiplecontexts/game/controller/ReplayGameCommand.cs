using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ReplayGameCommand : EventCommand
	{
		[Inject]
		public IScore scoreKeeper{get;set;}
		
		[Inject]
		public IGameTimer gameTimer{get;set;}
		
		[Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
		public IEventDispatcher crossContextDispatcher{get;set;}
		
		public override void Execute()
		{
			scoreKeeper.Reset();
			dispatcher.Dispatch(GameEvent.RESTART_GAME);
			crossContextDispatcher.Dispatch(GameEvent.RESTART_GAME);
			gameTimer.Start();
		}
	}
}

