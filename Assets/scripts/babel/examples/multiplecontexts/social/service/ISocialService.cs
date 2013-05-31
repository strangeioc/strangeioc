using System;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.examples.multiplecontexts.social
{
	public interface ISocialService
	{
		void FetchCurrentUser();
		void FetchScoresForFriends();
		IEventDispatcher dispatcher{get;set;}
	}
}

