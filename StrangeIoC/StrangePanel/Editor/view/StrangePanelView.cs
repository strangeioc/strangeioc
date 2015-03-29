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
 * @class strange.strangepanel.view.StrangePanelView
 * 
 * The main Inspector View for StrangePanel
 */

using System;
using strange.extensions.editor.impl;
using UnityEditor;
using UnityEngine;
using strange.strangepanel;
using strange.extensions.injector.api;
using UnityEditor.Callbacks;


namespace strange.strangepanel.view
{
	public class StrangePanelView : EditorView
	{
		[Inject]
		public TestSignal signal{ get; set; }


		string cameraNames = "";
		bool groupEnabled;
		bool myBool = true;
		float myFloat = 1.23f;

		Camera[] cameras;

		[MenuItem ("Window/StrangeIoC %.")]
		static void Init () 
		{
			// Get existing open window or if none, make a new one:
			StrangePanelView window = (StrangePanelView)EditorWindow.GetWindow (typeof (StrangePanelView));
			window.Show();
		}
		
		void OnGUI () 
		{
			GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
			cameraNames = EditorGUILayout.TextField ("Text Field", cameraNames);
			
			groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
			myBool = EditorGUILayout.Toggle ("Toggle", myBool);
			myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
			EditorGUILayout.EndToggleGroup ();

			if (GUILayout.Button ("test")) 
			{
				signal.Dispatch(myFloat);
			}


		}

		void OnInspectorGUI ()
		{
//			serializedObject.Update();
//			EditorGUILayout.PropertyField(serializedObject.FindProperty("integers"));
		}


		public void UpdateScripts ()
		{
			Debug.Log ("Update the Scripts");

			Camera[] cameras = FindObjectsOfType(typeof(Camera)) as Camera[];
			foreach (Camera camera in cameras) {
				cameraNames += camera.name;
			}
		}
	}
}

