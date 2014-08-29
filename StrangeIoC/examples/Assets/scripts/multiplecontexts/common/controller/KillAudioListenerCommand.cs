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

/// Turns off the AudioListener in this scene. Fired only if the present Context isn't firstContext

using System;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;

namespace strange.examples.multiplecontexts.common
{
	public class KillAudioListenerCommand : Command
	{
		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{ get; set; }

		public override void Execute()
		{
			AudioListener[] audioListeners = contextView.GetComponentsInChildren<AudioListener> ();
			int aa = audioListeners.Length;
			for (int a = 0; a < aa; a++)
			{
				audioListeners[a].enabled = false;
			}
		}
	}
}