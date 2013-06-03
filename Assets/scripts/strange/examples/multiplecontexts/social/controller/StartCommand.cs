/// StartCommand
/// ============================
/// This sets up the social component

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.social
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
			injectionBinder.Bind<UserVO>().ToValue(vo);
			
			dispatcher.Dispatch(SocialEvent.FULFILL_CURRENT_USER_REQUEST, vo);
			Release();
		}
	}
}

