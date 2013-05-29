using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.framework.impl;

namespace babel.extensions.context.impl
{
	public class Context : Binder, IContext
	{
		public object contextView{get;set;}
		
		public static IContext firstContext;
		
		public bool autoStartup;
		
		public Context ()
		{
		}
		
		public Context (object view, bool autoStartup)
		{
			if (firstContext == null)
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
		
		//Override to add componentry. Or just extend one of the provided extended Contexts
		virtual protected void addCoreComponents()
		{
		}
		
		//Override to instantiate componentry. Or just extend one of the provided extended Contexts
		virtual protected void instantiateCoreComponents()
		{
		}
		
		virtual public IContext SetContextView(object view)
		{
			contextView = view;
			return this;
		}
		
		virtual public IContext Start()
		{
			instantiateCoreComponents();
			mapBindings();
			postBindings();
			if (autoStartup)
				Launch();
			return this;
		}
		
		virtual public void Launch()
		{
		}
		
		//Override to map all your bindings
		virtual protected void mapBindings()
		{
		}
		
		//Allows overriders to do things after binding but before app launch
		virtual protected void postBindings()
		{
		}
		
		virtual public IContext AddContext(IContext context)
		{
			return this;
		}

		virtual public IContext RemoveContext(IContext context)
		{
			return this;
		}
	}
}

