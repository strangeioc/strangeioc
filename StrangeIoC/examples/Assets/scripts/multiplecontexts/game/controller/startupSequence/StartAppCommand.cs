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

/// Kicks off the app, directly after context binding

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.game
{
	public class StartAppCommand : Command
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		[Inject(ContextKeys.CONTEXT)]
		public IContext context{get;set;}
		
		public override void Execute()
		{
			//If we're not the first context, we need to shut down the AudioListener
			//This is one way to do it, but there is no "right" way
			if (context != Context.firstContext)
			{

			}
			
			
			//MonoBehaviours can only be injected after they've been instantiated manually.
			//Here we create the main GameLoop, attaching it to the ContextView.
			
			//Attach the GameLoop MonoBehaviour to the contextView...
			contextView.AddComponent<GameLoop>();
			IGameTimer timer = contextView.GetComponent<GameLoop>();
			//...then bind it for injection
			injectionBinder.Bind<IGameTimer>().ToValue(timer);
			
			GameObject go = new GameObject();
			go.name = "Scoreboard";
			go.AddComponent<ScoreboardView>();
			go.transform.parent = contextView.transform;
		}
	}
}

