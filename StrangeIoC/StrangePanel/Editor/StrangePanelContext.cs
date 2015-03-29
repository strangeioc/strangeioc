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
 * @class strange.strangepanel.StrangePanelContext
 * 
 * This example of EditorContext is both a tool to help you manage your
 * bindings in StrangeIoC from within the Unity Editor, and an example 
 * of how Strange can be used to author Unity Editor extensions.
 */

using System;
using strange.extensions.editor.impl;
using UnityEditor;
using UnityEngine;
using strange.strangepanel.controller;
using strange.strangepanel.service;
using strange.strangepanel.view;


namespace strange.strangepanel
{
	// InitializeOnLoad is a key piece of building an EditorView.
	// The Context initializes this way, without requiring anything like
	// the ContextView we normally rely upon.
	[InitializeOnLoad]
	public class StrangePanelContext : EditorMVCSContext
	{
		// This static Constructor is called because of the InitializeOnLoad
		// tag above. Use it to instantiate our Context.
		static StrangePanelContext ()
		{
			new StrangePanelContext();
		}

		protected override void mapBindings ()
		{
			base.mapBindings ();

			//Injections


			injectionBinder.Bind<ScriptReloadSignal>().ToSingleton ();
			injectionBinder.Bind<ScriptReloadService>().ToSingleton ();

			//Commands
			commandBinder.Bind<StartSignal> ().To<InitSystemCommand> ();
			commandBinder.Bind<TestSignal> ().To<TestCommand> ();

			//Mediation
			mediationBinder.Bind<StrangePanelView> ().To<StrangePanelMediator> ();
		}

		public override void Launch()
		{
			injectionBinder.GetInstance<StartSignal>().Dispatch ();
		}
	}
}

