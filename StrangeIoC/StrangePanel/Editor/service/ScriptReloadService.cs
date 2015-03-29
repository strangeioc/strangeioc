/*
 * Copyright 2015 StrangeIoC
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

/**
 * @class strange.strangepanel.service.ScriptReloadService
 * 
 * This service lets the rest of the Context know when Scripts have updated.
 * 
 * Notice how UnityEditor callbacks are often fixated around using statics.
 * They also tend to encourage you to mix View and Service logic.
 * We can't fix these parts of the engine, but we can use this type of
 * service to isolate such dubious practices.
 */

using System;
using UnityEditor.Callbacks;
using UnityEngine;


namespace strange.strangepanel.service
{
	public class ScriptReloadService
	{
		[Inject]
		public ScriptReloadSignal signal{ get; set; }

		private static ScriptReloadService instance;

		public ScriptReloadService()
		{
			if (instance == null) {
				instance = this;
			}
		}

		private void dispatch()
		{
			signal.Dispatch ();
		}
		

		[DidReloadScripts]
		static void DidReloadScripts()
		{
			instance.dispatch ();
		}

	}
}

