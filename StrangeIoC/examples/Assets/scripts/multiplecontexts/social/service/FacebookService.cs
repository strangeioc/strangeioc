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

/// This pretends to call Facebook and get you info relevant to the game
/// ======================
/// This is one of two example ISocialService classes, demonstrating how easily
/// you can swap between them thanks to IoC.
/// 
/// As I did in the myfirstproject example, note that I'm cheating a bit to show how
/// a webservice would work. You'd never really want to inject the contextView into your
/// service...I just want to demonstrate the async nature, so I'm borrowing a coroutine.
/// 
/// Never mind that the service is faked. What's important is that it's DIFFERENT from the
/// Google one. And you can swap the two of them in and own with a single-line
/// change in the Context that doesn't touch any other part of your app.

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.multiplecontexts.social
{
	public class FacebookService : ISocialService
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		[Inject]
		public IEventDispatcher dispatcher{get;set;}
		
		public FacebookService ()
		{
		}

		public void FetchCurrentUser()
		{
			MonoBehaviour root = contextView.GetComponent<SocialRoot>();
			root.StartCoroutine(waitASecondThenReturnCurrentUser());
		}

		public void FetchScoresForFriends()
		{
			MonoBehaviour root = contextView.GetComponent<SocialRoot>();
			root.StartCoroutine(waitASecondThenReturnFriendList());
		}

		private IEnumerator waitASecondThenReturnCurrentUser()
		{
			yield return new WaitForSeconds(1f);

			//...then pass back some fake data
			UserVO user = getUserData ("Zaphod", "12345", 
			                        "http://upload.wikimedia.org/wikipedia/en/7/72/Mark_Wing-Davey_as_Zaphod_Beeblebrox.jpg",
			                        100);

			dispatcher.Dispatch(SocialEvent.FULFILL_CURRENT_USER_REQUEST, user);
		}

		private IEnumerator waitASecondThenReturnFriendList()
		{
			yield return new WaitForSeconds(1f);

			ArrayList friends = new ArrayList ();

			friends.Add (getUserData("Arthur", "12346", "http://upload.wikimedia.org/wikipedia/en/e/eb/Arthur_Dent_Livid.jpg", 20));
			friends.Add (getUserData("Ford", "12347", "http://fc01.deviantart.net/fs7/i/2005/227/8/3/Ford_Prefect_by_KatoChan.jpg", 50));
			friends.Add (getUserData("Trillian", "12348", "http://upload.wikimedia.org/wikipedia/en/6/6d/Sandra_Dickinson_as_Trillian.jpg", 110));
			friends.Add (getUserData("Slartibartfast", "12349", "http://upload.wikimedia.org/wikipedia/en/3/31/SlartBartFast.JPG", 200));
			friends.Add (getUserData("Marvin", "12350", "http://upload.wikimedia.org/wikipedia/en/2/25/Marvin-TV-3.jpg", 800));

			dispatcher.Dispatch(SocialEvent.FULFILL_FRIENDS_REQUEST, friends);
		}

		private UserVO getUserData(string name, string id, string imgUrl, int score)
		{
			UserVO retv = new UserVO ();
			retv.userFirstName = name;
			retv.serviceId = id;
			retv.imgUrl = imgUrl;
			retv.highScore = score;
			return retv;
		}
	}
}

