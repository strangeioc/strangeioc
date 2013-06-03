/// Kicks off the app, directly after context binding

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.sequencer.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.game
{
	public class StartAppCommand : SequenceCommand
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
				AudioListener[] audioListeners = Camera.FindSceneObjectsOfType(typeof(AudioListener)) as AudioListener[];
				int aa = audioListeners.Length;
				for (int a = 1; a < aa; a++)
				{
					audioListeners[a].enabled = false;
				}
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

