/// The Root is the entry point to a Babel-enabled Unity3D app.
/// ===============
/// 
/// Attach this MonoBehaviour to a GameObject at the top of a scene in social.unity.
/// 
/// Social mocks a social component within your app, for example checking your high-score against
/// that of your Facebook friends.

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.context.impl;

namespace babel.examples.multiplecontexts.social
{
	public class SocialRoot : ContextView
	{
	
		void Awake()
		{
			//Instantiate the context, passing it this instance and a 'true' for autoStartup.
			context = new SocialContext(this, true);
			context.Start ();
		}
	}
}

