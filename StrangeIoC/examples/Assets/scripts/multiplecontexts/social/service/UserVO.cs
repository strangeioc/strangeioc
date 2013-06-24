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

/// User Value Object
/// =================
/// A Value Object (VO) is a simple class that contains properties and can be passed around.
/// These make great payloads to attach to the data property of an IEvent.
/// 
/// This UserVO carries some relevant data about the user as gleaned from a social service

using System;

namespace strange.examples.multiplecontexts.social
{
	public class UserVO
	{
		public string serviceId;
		public string imgUrl;
		public string userFirstName;
		public int highScore;
		public int currentScore;
	}
}

