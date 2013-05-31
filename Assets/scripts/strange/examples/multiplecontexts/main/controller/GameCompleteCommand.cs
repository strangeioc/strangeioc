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

