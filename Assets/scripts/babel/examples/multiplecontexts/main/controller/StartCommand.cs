/// An example Command
/// ============================
/// This Command puts a new ExampleView into the scene.
/// Note how the ContextView (i.e., the GameObject our Root was attached to)
/// is injected for use.
/// 
/// All Commands must override the Execute method. The Command is automatically
/// cleaned up when Execute has completed, unless Retain is called (more on that
/// in the OpenWebPageCommand).

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.command.impl;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.multiplecontexts.main
{
	public class StartCommand : Command
	{
		
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}
		
		public override void Execute()
		{
			GameObject go = new GameObject();
			go.name = "ExampleView";
			go.AddComponent<ExampleView>();
			go.transform.parent = contextView.transform;
		}
	}
}

