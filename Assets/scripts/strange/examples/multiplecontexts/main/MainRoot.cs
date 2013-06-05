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

/// The Root is the entry point to a strange-enabled Unity3D app.
/// ===============
/// 
/// Attach this MonoBehaviour to a GameObject at the top of a scene in main.unity.
/// 
/// Main is responsible for setting up the main context and loading the other components.

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;

namespace strange.examples.multiplecontexts.main
{
	public class MainRoot : ContextView
	{
	
		void Awake()
		{
			//Instantiate the context, passing it this instance and a 'true' for autoStartup.
			context = new MainContext(this, true);
			context.Start ();
		}
	}
}

