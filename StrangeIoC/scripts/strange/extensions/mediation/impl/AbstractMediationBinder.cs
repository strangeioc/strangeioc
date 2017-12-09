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
					case MediationEvent.ENABLED:
						EnableView (view, binding);
						break;
					case MediationEvent.DISABLED:
						DisableView (view, binding);
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

		/// Add a Mediator to a View. If the mediator is a "true" Mediator (i.e., it
		/// implements IMediator), perform PreRegister and OnRegister.
		protected virtual void ApplyMediationToView(IMediationBinding binding, IView view, Type mediatorType)
		{
			bool isTrueMediator = IsTrueMediator(mediatorType);
			if (!isTrueMediator || !HasMediator(view, mediatorType))
			{
				Type viewType = view.GetType();
				object mediator = CreateMediator(view, mediatorType);

				if (mediator == null)
					ThrowNullMediatorError (viewType, mediatorType);
				if (isTrueMediator)
					((IMediator)mediator).PreRegister();

				Type typeToInject = (binding.abstraction == null || binding.abstraction.Equals(BindingConst.NULLOID)) ? viewType : binding.abstraction as Type;
				injectionBinder.Bind(typeToInject).ToValue(view).ToInject(false);
				injectionBinder.injector.Inject(mediator);
				injectionBinder.Unbind(typeToInject);
				if (isTrueMediator)
				{
					((IMediator)mediator).OnRegister();
				}
			}
		}

		/// Add Mediators to Views. We make this virtual to allow for different concrete
		/// behaviors for different View/Mediation Types (e.g., MonoBehaviours require 
		/// different handling than EditorWindows)
		protected virtual void InjectViewAndChildren(IView view)
		{
			IView[] views = GetViews(view);
			int aa = views.Length;
			for (int a = aa - 1; a > -1; a--)
			{
				IView iView = views[a] as IView;
				if (iView != null && iView.shouldRegister)
				{
					if (iView.autoRegisterWithContext && iView.registeredWithContext)
					{
						continue;
					}
					iView.registeredWithContext = true;
					if (iView.Equals(view) == false)
						Trigger(MediationEvent.AWAKE, iView);
				}
			}
			injectionBinder.injector.Inject(view, false);
		}

		protected virtual bool IsTrueMediator(Type mediatorType)
		{
			return typeof(IMediator).IsAssignableFrom(mediatorType);
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
				binding = Bind (keyType);
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

		override protected Dictionary<string, object> ConformRuntimeItem(Dictionary<string, object> dictionary)
		{
			Dictionary<string, object> bindItems = new Dictionary<string, object> ();
			Dictionary<string, object> toItems = new Dictionary<string, object> ();
			foreach (var item in dictionary) 
			{
				if (item.Key == "BindView")
				{
					bindItems.Add ("Bind", item.Value);
				}
				else if (item.Key == "ToMediator")
				{
					toItems.Add ("To", item.Value);
				}
			}
			foreach (var item in bindItems)
			{
				dictionary.Remove ("BindView");
				dictionary.Add ("Bind", item.Value);
			}
			foreach (var item in toItems) 
			{
				dictionary.Remove ("ToMediator");
				dictionary.Add ("To", item.Value);
			}
			return dictionary;
		}

		override protected IBinding ConsumeItem(Dictionary<string, object> item, IBinding testBinding)
		{
			IBinding binding = base.ConsumeItem(item, testBinding);

			foreach (var i in item)
			{
				if (i.Key == "ToAbstraction")
				{
					Type abstractionType = Type.GetType (i.Value as string);
					IMediationBinding mediationBinding = (binding as IMediationBinding);
					if (abstractionType == null)
					{
						throw new BinderException ("A runtime abstraction in the MediationBinder returned a null Type. " + i.ToString(), BinderExceptionType.RUNTIME_NULL_VALUE);
					}
					if (mediationBinding == null)
					{
						throw new MediationException ("During an attempt at runtime abstraction a MediationBinding could not be found. " + i.ToString(), MediationExceptionType.BINDING_RESOLVED_TO_NULL);
					}

					mediationBinding.ToAbstraction (abstractionType);
				}
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

					if (view.enabled)
						EnableMediator(view, mediatorType);
				}
			}
		}

		/// Removes a mediator when its view is destroyed
		virtual protected void UnmapView(IView view, IMediationBinding binding)
		{
			TriggerInBindings(view, binding, DestroyMediator);
		}

		/// Enables a mediator when its view is enabled
		virtual protected void EnableView(IView view, IMediationBinding binding)
		{
			TriggerInBindings(view, binding, EnableMediator);
		}

		/// Disables a mediator when its view is disabled
		virtual protected void DisableView(IView view, IMediationBinding binding)
		{
			TriggerInBindings(view, binding, DisableMediator);
		}

		/// Triggers given function in all mediators bound to given view
		virtual protected void TriggerInBindings(IView view, IMediationBinding binding, Func<IView, Type, object> method)
		{
			Type viewType = view.GetType();

			if (bindings.ContainsKey(viewType))
			{
				object[] values = binding.value as object[];
				int aa = values.Length;
				for (int a = 0; a < aa; a++)
				{
					Type mediatorType = values[a] as Type;
					method (view, mediatorType);
				}
			}			
		}

		/// Create a new Mediator object based on the mediatorType on the provided view
		protected abstract object CreateMediator(IView view, Type mediatorType);

		/// Destroy the Mediator on the provided view object based on the mediatorType
		protected abstract IMediator DestroyMediator(IView view, Type mediatorType);

		/// Calls the OnEnabled method of the mediator
		protected abstract object EnableMediator(IView view, Type mediatorType);

		/// Calls the OnDisabled method of the mediator
		protected abstract object DisableMediator(IView view, Type mediatorType);

		/// Retrieve all views including children for this view
		protected abstract IView[] GetViews(IView view);

		/// Whether or not an instantiated Mediator of this type exists
		protected abstract bool HasMediator(IView view, Type mediatorType);

		/// Error thrown when a Mediator can't be instantiated
		/// Abstract because this happens for different reasons. Allow implementing
		/// class to specify the nature of the error.
		protected abstract void ThrowNullMediatorError(Type viewType, Type mediatorType);
	}
}

