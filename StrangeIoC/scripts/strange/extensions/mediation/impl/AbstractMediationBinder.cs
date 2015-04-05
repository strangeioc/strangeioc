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
 * Highest-level abstraction of the MediationBinder. Agnostic as to View and Mediator Type.
 * 
 * Please read strange.extensions.mediation.api.IMediationBinder
 * where I've extensively explained the purpose of View mediation
 */

using System;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.framework.api;
using strange.framework.impl;
using System.Collections.Generic;

namespace strange.extensions.mediation.impl
{
	abstract public class AbstractMediationBinder : Binder, IMediationBinder
	{

		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}

		public AbstractMediationBinder ()
		{
		}


		public override IBinding GetRawBinding ()
		{
			return new MediationBinding (resolver) as IBinding;
		}

		public void Trigger(MediationEvent evt, IView view)
		{
			Type viewType = view.GetType();
			IMediationBinding binding = GetBinding (viewType) as IMediationBinding;
			if (binding != null)
			{
				switch(evt)
				{
					case MediationEvent.AWAKE:
						InjectViewAndChildren(view);
						MapView (view, binding);
						break;
					case MediationEvent.DESTROYED:
						UnmapView (view, binding);
						break;
					default:
						break;
				}
			}
			else if (evt == MediationEvent.AWAKE)
			{
				//Even if not mapped, Views (and their children) have potential to be injected
				InjectViewAndChildren(view);
			}
		}

		override protected IBinding performKeyValueBindings(List<object> keyList, List<object> valueList)
		{
			IBinding binding = null;

			// Bind in order
			foreach (object key in keyList)
			{
				Type keyType = Type.GetType (key as string);
				if (keyType == null)
				{
					throw new BinderException ("A runtime Mediation Binding has resolved to null. Did you forget to register its fully-qualified name?\n View:" + key, BinderExceptionType.RUNTIME_NULL_VALUE);
				}
				if (binding == null)
				{
					binding = Bind (keyType);
				}
				else
				{
					binding = binding.Bind (keyType);
				}
			}
			foreach (object value in valueList)
			{
				Type valueType = Type.GetType (value as string);
				if (valueType == null)
				{
					throw new BinderException ("A runtime Mediation Binding has resolved to null. Did you forget to register its fully-qualified name?\n Mediator:" + value, BinderExceptionType.RUNTIME_NULL_VALUE);
				}
				binding = binding.To (valueType);
			}

			return binding;
		}

		new public IMediationBinding Bind<T> ()
		{
			return base.Bind<T> () as IMediationBinding;
		}

		public IMediationBinding BindView<T>()
		{
			return base.Bind<T> () as IMediationBinding;
		}

		/// Creates and registers one or more Mediators for a specific View instance.
		/// Takes a specific View instance and a binding and, if a binding is found for that type, creates and registers a Mediator.
		virtual protected void MapView(IView view, IMediationBinding binding)
		{
			Type viewType = view.GetType();

			if (bindings.ContainsKey (viewType))
			{
				object[] values = binding.value as object[];
				int aa = values.Length;
				for (int a = 0; a < aa; a++)
				{
					Type mediatorType = values [a] as Type;
					if (mediatorType == viewType)
					{
						throw new MediationException(viewType + "mapped to itself. The result would be a stack overflow.", MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);
					}
					ApplyMediationToView (binding, view, mediatorType);
				}
			}
		}

		/// Removes a mediator when its view is destroyed
		virtual protected void UnmapView(IView view, IMediationBinding binding)
		{
			Type viewType = view.GetType();

			if (bindings.ContainsKey(viewType))
			{
				object[] values = binding.value as object[];
				int aa = values.Length;
				for (int a = 0; a < aa; a++)
				{
					Type mediatorType = values[a] as Type;
					DestroyMediator (view, mediatorType);
				}
			}
		}

		/// Initialize all IViews within this view
		abstract protected void InjectViewAndChildren(IView view);

		/// Create a new Mediator object based on the mediatorType on the provided view
		abstract protected object CreateMediator(IView view, Type mediatorType);

		/// Destroy the Mediator on the provided view object based on the mediatorType
		abstract protected object DestroyMediator(IView view, Type mediatorType);

		/// Add Mediators to Views. We make this abstract to allow for different concrete
		/// behaviors for different View/Mediation Types (e.g., MonoBehaviours require 
		/// different handling than EditorWindows)
		abstract protected void ApplyMediationToView(IMediationBinding binding, IView view, Type mediatorType);
	}
}

