using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.examples.multiplecontexts.game
{
	public class GameLoop : MonoBehaviour, IGameTimer
	{
		private bool sendUpdates = false;
		
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{get;set;}
		
		public GameLoop ()
		{
		}
		
		public void Start()
		{
			sendUpdates = true;
		}
		
		public void Stop()
		{
			sendUpdates = false;
		}
		
		void Update()
		{
			if (sendUpdates && dispatcher != null)
				dispatcher.Dispatch(ExampleEvent.GAME_UPDATE);
		}
	}
}

