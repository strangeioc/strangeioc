using System;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.multiplecontexts.social
{
	public interface ISocialService
	{
		void FetchCurrentUser();
		void FetchScoresForFriends();
		IEventDispatcher dispatcher{get;set;}
	}
}

