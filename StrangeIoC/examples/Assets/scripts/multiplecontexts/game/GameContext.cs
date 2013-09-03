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

/// MainContext maps the Context for the top-level component.
/// ===========
/// I'm assuming here that you've already gone through myfirstproject, or that
/// you're experienced with strange.

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.game
{
	public class GameContext : MVCSContext
	{
		
		
		public GameContext () : base()
		{
		}
		
		public GameContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			injectionBinder.Bind<IScore>().To<ScoreModel>().ToSingleton();
			
			mediationBinder.Bind<ShipView>().To<ShipMediator>();
			mediationBinder.Bind<EnemyView>().To<EnemyMediator>();
			mediationBinder.Bind<ScoreboardView>().To<ScoreboardMediator>();
			
			commandBinder.Bind(ContextEvent.START).To<StartAppCommand>().To<StartGameCommand>().Once().InSequence();
			
			commandBinder.Bind(GameEvent.ADD_TO_SCORE).To<UpdateScoreCommand>();
			commandBinder.Bind(GameEvent.SHIP_DESTROYED).To<ShipDestroyedCommand>();
			commandBinder.Bind(GameEvent.GAME_OVER).To<GameOverCommand>();
			commandBinder.Bind(GameEvent.REPLAY).To<ReplayGameCommand>();
			commandBinder.Bind(GameEvent.REMOVE_SOCIAL_CONTEXT).To<RemoveSocialContextCommand>();
		}
	}
}

