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
 * @class strange.strangepanel.controller.InitSystemCommand
 * 
 * First thing to happen after the Context bootstraps.
 */


using System;
using strange.extensions.command.impl;
using UnityEngine;
using strange.strangepanel.service;

namespace strange.strangepanel.controller
{
	public class InitSystemCommand : Command
	{
		[Inject]	//We inject this here to force instantiation
		public ScriptReloadService scriptReloadService{ get; set; }
		
		public override void Execute ()
		{
		}
	}
}

