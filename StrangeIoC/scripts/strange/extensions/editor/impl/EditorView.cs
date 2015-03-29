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
 * @class strange.extensions.editor.impl.EditorView
 * 
 * An abstract class representing all Views in an Editor application
 */

using System;
using UnityEditor;
using UnityEngine;
using strange.extensions.mediation.api;


namespace strange.extensions.editor.impl
{
	abstract public class EditorView : EditorWindow, IView
	{
		
		protected EditorContext context;
		
		protected void OnEnable()
		{
			EditorContext.RegisterViewWithMatchingContext (this);
		}
		
		protected void OnDisable()
		{
			EditorContext.RemoveViewWithMatchingContext (this);
		}

		virtual protected void OnFocus()
		{
			if (requiresContext && !registeredWithContext)
			{
				Debug.LogWarning (this.GetType() + " requires a context but isn't registered with one!");
			}

		}

		/// Leave this value true most of the time. If for some reason you want
		/// a view to exist outside a context you can set it to false. The only
		/// difference is whether an error gets generated.
		private bool _requiresContext = true;
		public bool requiresContext
		{
			get
			{
				return _requiresContext;
			}
			set
			{
				_requiresContext = value;
			}
		}
		
		/// A flag for allowing the View to register with the Context
		/// In general you can ignore this. But some developers have asked for a way of disabling
		///  View registration with a checkbox from Unity, so here it is.
		/// If you want to expose this capability either
		/// (1) uncomment the commented-out line immediately below, or
		/// (2) subclass View and override the autoRegisterWithContext method using your own custom (public) field.
		//[SerializeField]
		protected bool registerWithContext = true;
		virtual public bool autoRegisterWithContext
		{
			get { return registerWithContext;  }
			set { registerWithContext = value; }
		}
		
		public bool registeredWithContext{get; set;}
	}
}

