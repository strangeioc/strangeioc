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

/**
 * @class strange.extensions.mediation.impl.Mediator
 * 
 * Base class for all Mediators.
 * 
 * @see strange.extensions.mediation.api.IMediationBinder
 */

using strange.extensions.context.api;
using strange.extensions.mediation.api;
using UnityEngine;

namespace strange.extensions.mediation.impl
{
	public class Mediator : MonoBehaviour, IMediator
	{

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}

		public Mediator ()
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

