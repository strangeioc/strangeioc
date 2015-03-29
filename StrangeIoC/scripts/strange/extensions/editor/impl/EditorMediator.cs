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
 * @class strange.extensions.editor.impl.EditorMediator
 * 
 * An implementation of IMediator particular to the requirements of Editor view mediation.
 */

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.mediation.api;


namespace strange.extensions.editor.impl
{
	public class EditorMediator : IMediator
	{

		public GameObject contextView {
			get {
				throw new EditorContextException("EditorMediators don't recognize a ContextView " + this, EditorContextExceptionType.CONTEXT_VIEW_UNSUPPORTED);
			}
			set {
				throw new EditorContextException("EditorMediators don't recognize a ContextView " + this, EditorContextExceptionType.CONTEXT_VIEW_UNSUPPORTED);
			}
		}
		
		public EditorMediator ()
		{
		}
		
		/**
		 * Fires directly after creation and before injection
		 */
		virtual public void PreRegister()
		{
		}
		
		/**
		 * Fires after all injections satisifed.
		 *
		 * Override and place your initialization code here.
		 */
		virtual public void OnRegister()
		{
		}
		
		/**
		 * Fires on removal of view.
		 *
		 * Override and place your cleanup code here
		 */
		virtual public void OnRemove()
		{
		}
	}
}
