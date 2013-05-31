/// Kicks off the app, directly after context binding

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.sequencer.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.game
{
	public class StartGameCommand : SequenceCommand
	{
		[Inject]
		public IGameTimer timer{get;set;}
		
		public override void Execute()
		{
			timer.Start();
		}
	}
}

