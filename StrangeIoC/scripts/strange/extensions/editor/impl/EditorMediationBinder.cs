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
 * @class strange.extensions.editor.impl.EditorMediationBinder
 * 
 * Binds EditorViews to EditorMediators.
 * 
 * Please read strange.extensions.mediation.api.IMediationBinder
 * where I've extensively explained the purpose of View mediation
 */

using System;
using System.Collections;
using UnityEngine;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.framework.api;
using strange.framework.impl;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

namespace strange.extensions.editor.impl
{
	public class EditorMediationBinder : Binder, IMediationBinder
	{
		
		[Inject]
		public IInjectionBinder injectionBinder{ get; set; }

		protected Dictionary<IBinding, List<IMediator>> mediators;
		
		public EditorMediationBinder ()
		{
			mediators = new Dictionary<IBinding,  List<IMediator>>();
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
					injectIntoView(view);
					mediate (view, binding);
					break;
				case MediationEvent.DESTROYED:
					unmediate (binding);
					break;
				default:
					break;
				}
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
		
		/// Perform injections on the view
		virtual protected void injectIntoView(IView view)
		{
			view.registeredWithContext = true;
			injectionBinder.injector.Inject (view, false);
		}
		
		new public IMediationBinding Bind<T> ()
		{
			return base.Bind<T> () as IMediationBinding;
		}
		
		public IMediationBinding BindView<T>() where T : MonoBehaviour
		{
			return base.Bind<T> () as IMediationBinding;
		}
		
		/// Creates and registers one or more Mediators for a specific View instance.
		/// Takes a specific View instance and a binding and, if a binding is found for that type, creates and registers a Mediator.
		virtual protected void mediate(IView view, IMediationBinding binding)
		{
			Type viewType = view.GetType();
			
			if (bindings.ContainsKey(viewType))
			{
				object[] values = binding.value as object[];
				for (int a = 0, aa = values.Length; a < aa; a++)
				{
					Type mediatorType = values [a] as Type;
					if (mediatorType == viewType)
					{
						throw new MediationException(viewType + "mapped to itself. The result would be a stack overflow.", MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);
					}

					//Bind the View
					Type typeToInject = (binding.abstraction == null || binding.abstraction.Equals(BindingConst.NULLOID)) ? viewType : binding.abstraction as Type;
					injectionBinder.Bind (typeToInject).ToValue (view).ToInject(false);

					//Bind/Instantiate/Preregister the Mediator
					injectionBinder.Bind(mediatorType).To (mediatorType);
					IMediator mediator = injectionBinder.GetInstance(mediatorType) as IMediator;
					mediator.PreRegister ();

					//Unbind View and Mediator
					injectionBinder.Unbind(mediatorType);
					injectionBinder.Unbind(typeToInject);
					mediator.OnRegister ();

					if (mediators.ContainsKey(binding) == false)
					{
						mediators[binding] = new List<IMediator>();
					}
					mediators[binding].Add(mediator);
				}
			}
		}
		
		/// Removes a mediator when its view is destroyed
		virtual protected void unmediate(IMediationBinding binding)
		{
			if (mediators.ContainsKey(binding))
			{
				List<IMediator> list = mediators[binding];
				foreach(IMediator mediator in list)
				{
					mediator.OnRemove();
				}
				mediators.Remove(binding);
			}
		}
	}
}

