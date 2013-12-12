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

/// SocialContext maps the Context for the social interactivity component.
/// ===========
/// A key thing to notice here is how easily we can swap one service (or model, or whatever)
/// for another that satisfies the same interface.

using System;
using UnityEngine;
using strange.examples.multiplecontexts.main;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.social
{
	public class SocialContext : MVCSContext
	{

		public SocialContext (MonoBehaviour view) : base(view)
		{
		}

		public SocialContext (MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
		{
		}
		
		protected override void mapBindings()
		{
			commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once();
			commandBinder.Bind(SocialEvent.FULFILL_CURRENT_USER_REQUEST).To<CreateUserTileCommand>();
				
			commandBinder.Bind(MainEvent.GAME_COMPLETE).InSequence()
				.To<GameCompleteCommand>()
				.To<CreateFriendListCommand>();
			
			commandBinder.Bind (MainEvent.REMOVE_SOCIAL_CONTEXT).To<RemoveContextCommand>();
			
			//So today we're posting to Facebook. Maybe tomorrow we'll want to use
			//GooglePlus, or Twitter, or Pinterest...
			injectionBinder.Bind<ISocialService> ().To<FacebookService> ().ToSingleton ();
			//injectionBinder.Bind<ISocialService> ().To<GoogleService> ().ToSingleton ();
			
			mediationBinder.Bind<UserTileView>().To<UserTileMediator>();
			mediationBinder.Bind<AwardView>().To<AwardViewMediator>();
		}
	}
}

