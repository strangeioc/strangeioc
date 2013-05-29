/// MainContext maps the Context for the top-level component.
/// ===========
/// I'm assuming here that you've already gone through myfirstproject, or that
/// you're experienced with Babel.

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.context.impl;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;

using babel.examples.multiplecontexts.game.util;
using babel.examples.multiplecontexts.game.view;
using babel.examples.multiplecontexts.game.controller;

namespace babel.examples.multiplecontexts.game
{
	public class GameContext : MVCSContext
	{
		
		
		public GameContext () : base()
		{
		}
		
		public GameContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			
			mediationBinder.Bind<ShipView>().To<ShipMediator>();
			mediationBinder.Bind<GamefieldView>().To<GamefieldMediator>();
			
			commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once();
		}
	}
}

