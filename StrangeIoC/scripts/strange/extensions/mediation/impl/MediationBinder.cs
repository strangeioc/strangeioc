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
using strange.framework.api;

namespace strange.extensions.mediation.impl
{
	public class MediationBinder : AbstractMediationBinder, IMediationBinder
	{

		public MediationBinder ()
		{
		}

		
		/// Initialize all IViews within this view
		protected override void InjectViewAndChildren(IView view)
		{
			MonoBehaviour mono = view as MonoBehaviour;
			Component[] views = mono.GetComponentsInChildren(typeof(IView), true) as Component[];
			
			int aa = views.Length;
			for (int a = aa - 1; a > -1; a--)
			{
				IView iView = views[a] as IView;
				if (iView != null)
				{
					if (iView.autoRegisterWithContext && iView.registeredWithContext)
					{
						continue;
					}
					iView.registeredWithContext = true;
					if (iView.Equals(mono) == false)
						Trigger (MediationEvent.AWAKE, iView);
				}
			}
			injectionBinder.injector.Inject (mono, false);

		}

		/// Add a Mediator to a View. If the mediator is a "true" Mediator (i.e., it
		/// implements IMediator, perform PreRegister and OnRegister.
		protected override void ApplyMediationToView(IMediationBinding binding, IView view, Type mediatorType)
		{
			bool isTrueMediator = mediatorType.IsAssignableFrom (typeof(IMediator));
			MonoBehaviour mono = view as MonoBehaviour;
			if (!isTrueMediator || mono.GetComponent (mediatorType) == null)
			{
				Type viewType = view.GetType();
				MonoBehaviour mediator = CreateMediator(view, mediatorType) as MonoBehaviour;
				if (mediator == null)
					throw new MediationException ("The view: " + viewType.ToString() + " is mapped to mediator: " + mediatorType.ToString() + ". AddComponent resulted in null, which probably means " + mediatorType.ToString().Substring(mediatorType.ToString().LastIndexOf(".")+1) + " is not a MonoBehaviour.", MediationExceptionType.NULL_MEDIATOR);
				if (isTrueMediator)
					((IMediator)mediator).PreRegister ();

				Type typeToInject = (binding.abstraction == null || binding.abstraction.Equals(BindingConst.NULLOID)) ? viewType : binding.abstraction as Type;
				injectionBinder.Bind (typeToInject).ToValue (view).ToInject(false);
				injectionBinder.injector.Inject (mediator);
				injectionBinder.Unbind(typeToInject);
				if (isTrueMediator)
				{
					((IMediator) mediator).OnRegister ();
				}
			}
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

