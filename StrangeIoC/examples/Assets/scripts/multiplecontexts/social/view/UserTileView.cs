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

/// User tile view
/// ==========================
/// 

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

namespace strange.examples.multiplecontexts.social
{
	public class UserTileView : View
	{
		internal const string CLICK_EVENT = "CLICK_EVENT";
		
		[Inject]
		public IEventDispatcher dispatcher{get;set;}
		
		private string imgUrl;
		private UserVO userVO;
		
		//Publicly settable from Unity3D
		public GameObject edx_ImageHolder;
		public TextMesh edx_UserName;
		public TextMesh edx_Score;
		
		private Vector3 dest;
		
		internal void init()
		{
		}
		
		public void setUser(UserVO vo)
		{
			if (userVO == null || vo.serviceId == userVO.serviceId)
			{
				userVO = vo;
				updateImage(userVO.imgUrl);
				updateName(userVO.userFirstName);
				updateScore(userVO.highScore);
			}
		}
		
		public UserVO getUser()
		{
			return userVO;
		}
		
		public void SetTilePosition(Vector3 dest)
		{
			this.dest = dest;
			StartCoroutine(tweenToPosition());
		}
		
		private IEnumerator tweenToPosition()
		{
			Vector3 pos = gameObject.transform.localPosition;
			
			while (Vector3.Distance(pos, dest) > .1f)
			{
				pos += (dest - pos) * .09f;
				gameObject.transform.position = pos;
				yield return null;
			}
			gameObject.transform.position = dest;
		}
		
		private void updateImage(string url)
		{
			if (url == imgUrl)
			{
				return;
			}
			
			imgUrl = url;
			if (!String.IsNullOrEmpty(imgUrl))
			{
				StartCoroutine(loadUserImg());
			}
		}
		
		private IEnumerator loadUserImg()
		{
			WWW www = new WWW(imgUrl);
			yield return www;
			edx_ImageHolder.renderer.material.mainTexture = www.texture;
		}
		
		internal void updateName(string name)
		{
			edx_UserName.text = name;
		}
		
		internal void updateScore(int score)
		{
			edx_Score.text = score.ToString();
		}
	}
}

