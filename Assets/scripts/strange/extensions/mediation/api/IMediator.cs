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
 * @interface strange.extensions.mediation.api.IMediator
 * 
 * Look at strange.extensions.mediation.api.IMediationBinder,
 * where I explain the purpose of Mediation in detail.
 * 
 * @see strange.extensions.mediation.api.IMediationBinder
 */

using System;
using UnityEngine;

namespace strange.extensions.mediation.api
{
	public interface IMediator
	{
		/// Get/set the GameObject that represents the top-most item in this Context
		GameObject contextView {get;set;}

		/// This method fires immediately after instantiation, but before injection.
		/// Override to handle anything that needs to happen prior to injection.
		void PreRegister();

		/// This method fires immediately after injection.
		/// Override to perform the actions you might normally perform in a constructor.
		void OnRegister();

		/// This method fires just before a GameObject will be destroyed.
		/// Override to clean up any listeners, or anything else that might keep the View/Mediator pair from being garbage collected.
		void OnRemove();
	}
}

