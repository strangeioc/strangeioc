/// StartCommand
/// ============================
/// This sets up the social component

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.command.impl;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.social
{
	public class StartCommand : EventCommand
	{
		
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		[Inject]
		public ISocialService social{get;set;}
		
		public override void Execute()
		{
			Retain();
			//Note how we're using the same event for convenience here
			//and below. But the local event bus and the global one are separate, so there's
			//no systemic confusion.
			social.dispatcher.addListener(SocialEvent.FULFILL_CURRENT_USER_REQUEST, onResponse);
			social.FetchCurrentUser();
		}
		
		private void onResponse(object payload)
		{
			social.dispatcher.removeListener(SocialEvent.FULFILL_CURRENT_USER_REQUEST, onResponse);
			TmEvent evt = payload as TmEvent;
			UserVO vo = evt.data as UserVO;
			
			//We're going to Bind this for injection, since we'll need it later when we compare
			//the user's highscore with his own score and the highscore of others.
			injectionBinder.Bind<UserVO>().AsValue(vo);
			
			dispatcher.Dispatch(SocialEvent.FULFILL_CURRENT_USER_REQUEST, vo);
			Release();
		}
	}
}

