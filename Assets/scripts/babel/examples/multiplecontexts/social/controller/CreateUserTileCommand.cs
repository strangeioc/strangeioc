/// CreateUserTileCommand
/// ============================
/// Creates the tile that represents the user

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.command.impl;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.social
{
	public class CreateUserTileCommand : EventCommand
	{
		
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		public override void Execute()
		{
			UserVO vo = evt.data as UserVO;
			
			GameObject go = UnityEngine.Object.Instantiate(Resources.Load("GameTile")) as GameObject;
			go.transform.parent = contextView.transform;
			go.AddComponent<UserTileView>();
			
			//Here's something interesting. I'm technically bypassing the mediator here.
			//Stylistically I think this is fine during instantiation. Your team might decide differently.
			UserTileView view = go.GetComponent<UserTileView>() as UserTileView;
			view.setUser(vo);
			
			Vector3 bottomLeft = new Vector3(.1f, .1f, (Camera.main.farClipPlane - Camera.main.nearClipPlane)/2f);
			Vector3 dest = Camera.main.ViewportToWorldPoint(bottomLeft);
			view.SetTilePosition(dest);
		}
	}
}

