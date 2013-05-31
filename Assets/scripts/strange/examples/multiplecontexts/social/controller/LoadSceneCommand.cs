/// LoadSceneCommand
/// ============================
/// This Command adds a Scene to the current one

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.multiplecontexts.social
{
	public class LoadSceneCommand : EventCommand
	{
		
		public override void Execute()
		{
			string filepath = evt.data as string;
			
			//Load the component
			if (String.IsNullOrEmpty(filepath))
			{
				throw new Exception("Can't load a module with a null or empty filepath.");
			}
			Application.LoadLevelAdditive(filepath);
		}
	}
}

