/// The Root is the entry point to a strange-enabled Unity3D app.
/// ===============
/// 
/// Attach this MonoBehaviour to a GameObject at the top of a scene in game.unity.
/// 
/// Game includes a simple Asteroids-style game.

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace strange.examples.multiplecontexts.game
{
	public class GameRoot : ContextView
	{
	
		void Awake()
		{
			//Instantiate the context, passing it this instance and a 'true' for autoStartup.
			context = new GameContext(this, true);
			context.Start ();
		}
	}
}

