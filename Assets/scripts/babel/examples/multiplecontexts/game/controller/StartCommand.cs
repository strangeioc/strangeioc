/// Kicks off the app, directly after context binding

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.command.impl;
using babel.extensions.dispatcher.eventdispatcher.impl;

using babel.examples.multiplecontexts.game.util;
using babel.examples.multiplecontexts.game.view;

namespace babel.examples.multiplecontexts.game
{
	public class StartCommand : Command
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		public override void Execute()
		{
			//MonoBehaviours can only be injected after they've been instantiated manually.
			//Here we create the main GameLoop, attaching it to the ContextView.
			//In order for it to be Injected, 
			
			//Attach the GameLoop MonoBehaviour to the contextView...
			contextView.AddComponent<GameLoop>();
			IGameTimer timer = contextView.GetComponent<GameLoop>();
			//...then bind it for injection
			injectionBinder.Bind<IGameTimer>().AsValue(timer);
			injectionBinder.GetInstance<IGameTimer>();

			timer.Start();
		}
	}
}

