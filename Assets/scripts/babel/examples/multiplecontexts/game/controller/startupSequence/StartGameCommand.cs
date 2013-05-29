/// Kicks off the app, directly after context binding

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.sequencer.impl;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.game
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

