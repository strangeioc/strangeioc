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
 * @class strange.extensions.context.impl.MVCSContext
 * 
 * The recommended Context to extend for using Strange for Unity Editor plugins.
 * 
 * TODO: DOCUMENTATION
 */
using System;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.implicitBind.api;
using strange.extensions.implicitBind.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using strange.framework.api;

namespace strange.extensions.editor.impl
{
	public class EditorMVCSContext : EditorContext
	{
		// A Binder that maps Interfaces and abstract classes to concrete implementations
		public IInjectionBinder injectionBinder;

		/// A Binder that maps Signals to Commands
		public ICommandBinder commandBinder{get;set;}
		
		/// A Binder that serves as an alternate, event-based bus for the Context
		public IEventDispatcher dispatcher{get;set;}
		
		/// A Binder that maps Views to Mediators
		public IMediationBinder mediationBinder{get;set;}
		
		//Interprets implicit bindings
		public IImplicitBinder implicitBinder { get; set; }
		
		
		/// A list of Views Awake before the Context is fully set up.
		//protected static ISemiBinding viewCache = new SemiBinding();
		
		/// Map the relationships between the Binders.
		/// Although you can override this method, it is recommended
		/// that you provide all your application bindings in `mapBindings()`.
		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			injectionBinder = new InjectionBinder ();

			injectionBinder.Bind<IInstanceProvider>().Bind<IInjectionBinder>().ToValue(injectionBinder);
			injectionBinder.Bind<IContext>().ToValue(this).ToName(ContextKeys.CONTEXT);
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
			//This binding is for local event dispatchers
			//You can disable it if you plan to use Signals exclusively
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>();
			//This binding is for the event system bus
			//You can disable it if you plan to use Signals exclusively
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().ToSingleton().ToName(ContextKeys.CONTEXT_DISPATCHER);
			injectionBinder.Bind<IMediationBinder>().To<EditorMediationBinder>().ToSingleton();
			injectionBinder.Bind<IImplicitBinder>().To<ImplicitBinder>().ToSingleton();
		}
		
		protected override void instantiateCoreComponents()
		{
			base.instantiateCoreComponents();

			commandBinder = injectionBinder.GetInstance<ICommandBinder>() as ICommandBinder;
			
			dispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			mediationBinder = injectionBinder.GetInstance<IMediationBinder>() as IMediationBinder;
			implicitBinder = injectionBinder.GetInstance<IImplicitBinder>() as IImplicitBinder;
			
			(dispatcher as ITriggerProvider).AddTriggerable(commandBinder as ITriggerable);
		}
		
		protected override void postBindings()
		{
			//It's possible for views to fire their Awake before bindings. This catches any early risers and attaches their Mediators.
			//mediateViewCache();
		}
		
		/// Fires the start Signal.
		/// You need to:
		/// 1. Define a StartSignal in your own code,
		/// 2. Map that Signal to the first Command you want to run, and
		/// 3. Override this method and issue StartSignal.Dispatch().
		/// 
		/// We don't do any of this for you, because you might want a payload in your StartSignal,
		/// which requires defining your own.
		public override void Launch()
		{
			throw new EditorContextException ("You need a Launch method in your Context!", EditorContextExceptionType.ABSTRACT_METHOD);
		}
		
		/// Gets an instance of the provided generic type.
		/// Always bear in mind that maintaining such a reference risks 
		/// adding a dependency that must be cleaned up when Contexts
		/// are removed.
		override public object GetComponent<T>()
		{
			return GetComponent<T>(null);
		}
		
		/// Gets an instance of the provided generic type and name from the InjectionBinder
		/// Always bear in mind that maintaining such a reference risks 
		/// adding a dependency that must be cleaned up when Contexts
		/// are removed.
		override public object GetComponent<T>(object name)
		{
			IInjectionBinding binding = injectionBinder.GetBinding<T>(name);
			if (binding != null)
			{
				return injectionBinder.GetInstance<T>(name);
			}
			return null;
		}
		
		override public void AddView(object view)
		{
			if (mediationBinder != null)
			{
				mediationBinder.Trigger(MediationEvent.AWAKE, view as IView);
			}
			else
			{
				//cacheView(view);
			}
		}
		
		override public void RemoveView(object view)
		{
			mediationBinder.Trigger(MediationEvent.DESTROYED, view as IView);
		}

		//TODO: WE CAN PROBABLY REMOVE ALL LOGIC RELATED TO EARLY RISERS. I
		//SUSPECT THIS ISN'T AN ISSUE WITH EDITOR VIEWS.


		/// Caches early-riser Views.
		/// 
		/// If a View is on stage at startup, it's possible for that
		/// View to be Awake before this Context has finished initing.
		/// `cacheView()` maintains a list of such 'early-risers'
		/// until the Context is ready to mediate them.
//		virtual protected void cacheView(MonoBehaviour view)
//		{
//			if (viewCache.constraint.Equals(BindingConstraintType.ONE))
//			{
//				viewCache.constraint = BindingConstraintType.MANY;
//			}
//			viewCache.Add(view);
//		}
		
		/// Provide mediation for early-riser Views
//		virtual protected void mediateViewCache()
//		{
//			if (mediationBinder == null)
//				throw new ContextException("EditorMVCSContext cannot mediate views without a mediationBinder", ContextExceptionType.NO_MEDIATION_BINDER);
//			
//			object[] values = viewCache.value as object[];
//			if (values == null)
//			{
//				return;
//			}
//			int aa = values.Length;
//			for (int a = 0; a < aa; a++)
//			{
//				mediationBinder.Trigger(MediationEvent.AWAKE, values[a] as IView);
//			}
//			viewCache = new SemiBinding();
//		}
		
		/// Clean up. Called by a ContextView in its OnDestroy method
		/// TODO: does this apply here? Do Editor Contexts need cleanup of this type?
		public override void OnRemove()
		{
			base.OnRemove();
			commandBinder.OnRemove();
		}
		
		/// <summary>
		/// Determines whether this Context has mapping for view the specified View.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="view">An IView for use with an Editor</param>
		override public bool HasMappingForView (IView view)
		{
			Type viewType = view.GetType();
			return mediationBinder.GetBinding(viewType) != null;
		}
		
		/// <summary>
		/// Registers the View with the Context.
		/// </summary>
		/// <param name="view">An IView for use with an Editor</param>
		override public void RegisterView(IView view)
		{
			mediationBinder.Trigger (MediationEvent.AWAKE, view);
		}
	}
}



