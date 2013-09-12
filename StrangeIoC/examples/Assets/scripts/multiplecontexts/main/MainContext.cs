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
using strange.examples.multiplecontexts.game;

namespace strange.examples.multiplecontexts.main
{
	public class MainContext : MVCSContext
	{
		
		public MainContext () : base()
		{
		}
		
		public MainContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			//Any event that fire across the Context boundary get mapped here.
			crossContextBridge.Bind(MainEvent.GAME_COMPLETE);
			crossContextBridge.Bind(MainEvent.REMOVE_SOCIAL_CONTEXT);
			crossContextBridge.Bind(GameEvent.RESTART_GAME);
			
			commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once();
			commandBinder.Bind(MainEvent.LOAD_SCENE).To<LoadSceneCommand>();
			
			commandBinder.Bind (MainEvent.GAME_COMPLETE).To<GameCompleteCommand>();
			
			//I'm not actually doing this anywhere in this example (probably I'll add something soon)
			//but here's how you'd cross-context map a shared model or service
			//injectionBinder.Bind<ISomeInterface>().To<SomeInterfaceImplementer>().ToSingleton().CrossContext();
		}
	}
}

