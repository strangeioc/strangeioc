/*
 * Copyright 2015, StrangeIoC
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
 * @class strange.extensions.context.impl.EditorContext
 * 
 * The EditorContext is the entry point to the binding framework 
 * for Editor-based applications.
 * 
 * Extend this class to create a binding context suitable 
 * for your Unity editor plugin.
 * 
 * Note that EditorContexts allow only a single Context and have
 * no notion of a ContextView (these concepts aren't really
 * meaningful within the Editor).
 * 
 */

using strange.extensions.context.api;
using strange.framework.impl;
using strange.extensions.context.impl;
using strange.extensions.mediation.api;
using System.Collections.Generic;

namespace strange.extensions.editor.impl
{
	//Place the InitializeOnLoad attribute at the head of
	//your subclass.
	//[InitializeOnLoad]
	public class EditorContext : Binder, IContext, IEditorContext
	{

		public static List<IEditorContext> contexts = new List<IEditorContext>();
		
		/// If false, the `Launch()` method won't fire.
		public bool autoStartup;



		//Your subclass must use a static instance and static Constructor
		//in order to be instantiated at startup
		
//		protected static EditorContextSubclass instance;
		
//		static EditorContextSubclass ()
//		{
//			if (instance == null) {
//				instance = new EditorContextSubclass();
//			}
//		}

		public EditorContext () : this (ContextStartupFlags.AUTOMATIC){}
		
		public EditorContext (ContextStartupFlags flags)
		{
			contexts.Add(this);

			addCoreComponents();
			this.autoStartup = (flags & ContextStartupFlags.MANUAL_LAUNCH) != ContextStartupFlags.MANUAL_LAUNCH;
			if ((flags & ContextStartupFlags.MANUAL_MAPPING) != ContextStartupFlags.MANUAL_MAPPING)
			{
				Start();
			}
		}
		
		public EditorContext (object view) : this (ContextStartupFlags.AUTOMATIC){}
		
		public EditorContext (object view, bool autoMapping) : this(autoMapping ? ContextStartupFlags.MANUAL_MAPPING : ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING)
		{
		}
		
		/// Override to add componentry. Or just extend MVCSContext.
		virtual protected void addCoreComponents()
		{
		}
		
		/// Override to instantiate componentry. Or just extend MVCSContext.
		virtual protected void instantiateCoreComponents()
		{
		}
		
		virtual public object GetContextView() 
		{
			throw new EditorContextException ("The EditorContext does not support the notion of a ContextView.", EditorContextExceptionType.CONTEXT_VIEW_UNSUPPORTED);
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

		/// <summary>
		/// Looks for a Context that declares this View, and registers with it.
		/// </summary>
		/// <param name="view">An View for use with the Editor</param>
		static public void RegisterViewWithMatchingContext(IView view)
		{
			foreach (IEditorContext context in contexts)
			{
				if (context.HasMappingForView(view))
				{
					context.RegisterView(view);
				}
			}
		}

		/// <summary>
		/// Looks for a Context that declares this View, and Unregisters from it.
		/// </summary>
		/// <param name="view">An View for use with the Editor</param>
		static public void RemoveViewWithMatchingContext(IView view)
		{
			foreach (IEditorContext context in contexts)
			{
				if (context.HasMappingForView(view))
				{
					(context as IContext).RemoveView(view);
				}
			}
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

		/// <summary>
		/// Determines whether this Context has mapping for view the specified View.
		/// Override in subclass.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="view">An IView for use with an Editor</param>
		virtual public bool HasMappingForView (IView view)
		{
			return false;
		}
		
		/// <summary>
		/// Registers the View with the Context.
		/// Override in subclass.
		/// </summary>
		/// <param name="view">An IView for use with an Editor</param>
		virtual public void RegisterView(IView view)
		{
			throw new EditorContextException("EditorContext does not allow RegisterView. Implement this method in a subclass.", EditorContextExceptionType.CONTEXT_VIEW_UNSUPPORTED);
		}
	}
}

