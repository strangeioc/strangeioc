/// User Value Object
/// =================
/// A Value Object (VO) is a simple class that contains properties and can be passed around.
/// These make great payloads to attach to the data property of TmEvent.
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

