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

/// GameCompleteCommand
/// ============================
/// This Command demonstrates that the main context has received the GameComplete event

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.main
{
	public class GameCompleteCommand : EventCommand
	{
		
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		public override void Execute()
		{
			int score = (int)evt.data;
			
			Debug.Log ("MAIN SCENE KNOWS THAT GAME IS OVER. Your score is: " + score);
		}
	}
}

