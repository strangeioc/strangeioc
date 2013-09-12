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
using strange.examples.multiplecontexts.main;
using strange.extensions.command.impl;

namespace strange.examples.multiplecontexts.game
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
			
			dispatcher.Dispatch(MainEvent.GAME_COMPLETE, scoreKeeper.score);
		}
	}
}

