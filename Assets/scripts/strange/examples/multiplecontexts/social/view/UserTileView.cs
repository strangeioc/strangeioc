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

