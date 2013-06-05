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

