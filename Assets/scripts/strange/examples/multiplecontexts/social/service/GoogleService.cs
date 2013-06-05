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

/// This pretends to call GooglePlus and get you info relevant to the game
/// ======================
/// This is one of two example ISocialService classes, demonstrating how easily
/// you can swap between them thanks to IoC.
/// 
/// As I did in the myfirstproject example, note that I'm cheating a bit to show how
/// a webservice would work. You'd never really want to inject the contextView into your
/// service...I just want to demonstrate the async nature, so I'm borrowing a coroutine.
/// 
/// Never mind that the service is faked. What's important is that it's DIFFERENT from the
/// Facebook one. And you can swap the two of them in and own with a single-line
/// change in the Context that doesn't touch any other part of your app.

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.examples.multiplecontexts.social
{
	public class GoogleService : ISocialService
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}

		[Inject]
		public IEventDispatcher dispatcher{get;set;}

		public GoogleService ()
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
			UserVO user = getUserData ("Kirk", "54321", 
			                           "http://upload.wikimedia.org/wikipedia/commons/a/a5/Star_Trek_William_Shatner.JPG",
			                           100);

			dispatcher.Dispatch(SocialEvent.FULFILL_CURRENT_USER_REQUEST, user);
		}

		private IEnumerator waitASecondThenReturnFriendList()
		{
			yield return new WaitForSeconds(1f);

			ArrayList friends = new ArrayList ();

			friends.Add (getUserData("Spock", "54322", "http://upload.wikimedia.org/wikipedia/commons/a/a8/Leonard_Nimoy_William_Shatner_Star_Trek_1968.JPG", 20));
			friends.Add (getUserData("McCoy", "54323", "http://upload.wikimedia.org/wikipedia/commons/6/6a/Deforest_Kelly_Dr_McCoy_Star_Trek.JPG", 50));
			friends.Add (getUserData("Uhura", "54324", "http://upload.wikimedia.org/wikipedia/commons/b/b7/Nichelle_Nichols%2C_NASA_Recruiter_-_GPN-2004-00017.jpg", 110));
			friends.Add (getUserData("Sulu", "54325", "http://upload.wikimedia.org/wikipedia/commons/f/f8/George_Takei_Sulu_Star_Trek.JPG", 200));
			friends.Add (getUserData("Khaaan!", "54326", "http://upload.wikimedia.org/wikipedia/en/1/19/Khan1.jpg", 800));

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

