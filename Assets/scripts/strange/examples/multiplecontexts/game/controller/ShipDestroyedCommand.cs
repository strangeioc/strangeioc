using System;
using UnityEngine;
using strange.extensions.command.impl;

namespace strange.examples.multiplecontexts.game
{
	public class ShipDestroyedCommand : EventCommand
	{
		[Inject]
		public IScore scoreKeeper{get;set;}
		
		public override void Execute()
		{
			int livesRemaining = scoreKeeper.LoseLife();
			dispatcher.Dispatch(GameEvent.LIVES_CHANGE, livesRemaining);
			
			if (livesRemaining == 0)
			{
				dispatcher.Dispatch(GameEvent.GAME_OVER);
			}
		}
	}
}

