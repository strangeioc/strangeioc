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
 * @class strange.extensions.context.impl.Context
 * 
 * A Context is the entry point to the binding framework.
 * 
 * Extend this class to create the binding context suitable 
 * for your application.
 * 
 * In a typical Unity3D setup, extend MVCSContext and instantiate 
 * your extension from the ContextView.
 */

using strange.extensions.context.api;
using strange.framework.impl;

namespace strange.extensions.context.impl
{
	public class Context : Binder, IContext
	{
		/// The top of the View hierarchy.
		/// In MVCSContext, this is your top-level GameObject
		public object contextView{get;set;}

		/// In a multi-Context app, this represents the first Context to instantiate.
		public static IContext firstContext;

		/// If false, the `Launch()` method won't fire.
		public bool autoStartup;
		
		public Context ()
		{
		}
		
		public Context (object view, bool autoStartup)
		{
			//If firstContext was unloaded, the contextView will be null. Assign the new context as firstContext.
			if (firstContext == null || firstContext.GetContextView() == null)
			{
				firstContext = this;
			}
			else
			{
				firstContext.AddContext(this);
			}
			this.autoStartup = autoStartup;
			SetContextView(view);
			addCoreComponents();
		}
		
		/// Override to add componentry. Or just extend MVCSContext.
		virtual protected void addCoreComponents()
		{
		}
		
		/// Override to instantiate componentry. Or just extend MVCSContext.
		virtual protected void instantiateCoreComponents()
		{
		}

		/// Set the object that represents the top of the Context hierarchy.
		/// In MVCSContext, this would be a GameObject.
		virtual public IContext SetContextView(object view)
		{
			contextView = view;
			return this;
		}
		
		virtual public object GetContextView() 
		{ 
			return contextView; 
		}

		/// Call this from your Root to set everything in action.
		virtual public IContext Start()
		{
			instantiateCoreComponents();
			mapBindings();
			postBindings();
			if (autoStartup)
				Launch();
			return this;
		}

		/// The final method to fire after mappings.
		/// If autoStartup is false, you need to call this manually.
		virtual public void Launch()
		{
		}
		
		/// Override to map project-specific bindings
		virtual protected void mapBindings()
		{
		}
		
		/// Override to do things after binding but before app launch
		virtual protected void postBindings()
		{
		}

		/// Add another Context to this one.
		virtual public IContext AddContext(IContext context)
		{
			return this;
		}

		/// Remove a context from this one.
		virtual public IContext RemoveContext(IContext context)
		{
			context.OnRemove();
			return this;
		}

		/// Retrieve a component from this Context by generic type
		virtual public object GetComponent<T>()
		{
			return null;
		}


		/// Retrieve a component from this Context by generic type and name
		virtual public object GetComponent<T>(object name)
		{
			return null;
		}

		/// Register a View with this Context
		virtual public void AddView(object view)
		{
			//Override in subclasses
		}

		/// Remove a View from this Context
		virtual public void RemoveView(object view)
		{
			//Override in subclasses
		}

	}
}

