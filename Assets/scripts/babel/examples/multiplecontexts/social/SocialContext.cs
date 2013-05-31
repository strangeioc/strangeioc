/// SocialContext maps the Context for the social interactivity component.
/// ===========
/// A key thing to notice here is how easily we can swap one service (or model, or whatever)
/// for another that satisfies the same interface.

using System;
using UnityEngine;
using babel.examples.multiplecontexts.main;
using babel.extensions.context.api;
using babel.extensions.context.impl;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.social
{
	public class SocialContext : MVCSContext
	{
		
		public SocialContext () : base()
		{
		}
		
		public SocialContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once();
			commandBinder.Bind(SocialEvent.FULFILL_CURRENT_USER_REQUEST).To<CreateUserTileCommand>();
				
			sequencer.Bind(MainEvent.GAME_COMPLETE)
				.To<GameCompleteCommand>()
				.To<CreateFriendListCommand>();
			
			//So today we're posting to Facebook. Maybe tomorrow we'll want to use
			//GooglePlus, or Twitter, or Pinterest...
			injectionBinder.Bind<ISocialService> ().To<FacebookService> ().AsSingleton ();
			//injectionBinder.Bind<ISocialService> ().To<GoogleService> ().AsSingleton ();
			
			mediationBinder.Bind<UserTileView>().To<UserTileMediator>();
			mediationBinder.Bind<AwardView>().To<AwardViewMediator>();
		}
	}
}

