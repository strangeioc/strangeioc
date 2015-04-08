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
 * @class strange.extensions.mediation.impl.MediationBinder
 * 
 * Binds Views to Mediators. This is the base class for all MediationBinders
 * that work with MonoBehaviours.
 * 
 * Please read strange.extensions.mediation.api.IMediationBinder
 * where I've extensively explained the purpose of View mediation
 */

using System;
using UnityEngine;
using strange.extensions.mediation.api;

namespace strange.extensions.mediation.impl
{
	public class MediationBinder : AbstractMediationBinder, IMediationBinder
	{

		public MediationBinder ()
		{
		}

		protected override IView[] GetViews(IView view)
		{
			MonoBehaviour mono = view as MonoBehaviour;
			return mono.GetComponentsInChildren(typeof(IView), true) as IView[];
		}

		protected override bool HasMediator(IView view, Type mediatorType)
		{
			MonoBehaviour mono = view as MonoBehaviour;
			return mono.GetComponent(mediatorType) != null;
		}

		/// Create a new Mediator object based on the mediatorType on the provided view
		protected override object CreateMediator(IView view, Type mediatorType)
		{
			MonoBehaviour mono = view as MonoBehaviour;
			return mono.gameObject.AddComponent(mediatorType);
		}

		/// Destroy the Mediator on the provided view object based on the mediatorType
		protected override object DestroyMediator(IView view, Type mediatorType)
		{
			MonoBehaviour mono = view as MonoBehaviour;
			IMediator mediator = mono.GetComponent(mediatorType) as IMediator;
			if (mediator != null)
			{
				DestroyMediator (mediator);
			}
			return DestroyMediator (mediator);
		}

		/// Destroy the provided Mediator
		protected object DestroyMediator(IMediator mediator)
		{
			mediator.OnRemove();
			return mediator;
		}
	}
}

