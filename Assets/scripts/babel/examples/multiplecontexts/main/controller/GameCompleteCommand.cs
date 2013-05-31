/// GameCompleteCommand
/// ============================
/// This Command demonstrates that the main context has received the GameComplete event

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.command.impl;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.main
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

