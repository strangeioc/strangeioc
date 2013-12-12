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
 * @class strange.extensions.context.impl.CrossContext
 * 
 * Provides the capabilities that allow a Context to communicate across
 * the Context boundary. Specifically, CrossContext provides
 * - A CrossContextInjectionBinder that allows injections to be shared cross-context
 * - An EventDispatcher that allows messages to be sent between Contexts
 * - Methods (the ICrossContextCapable API) for adding and removing the hooks between Contexts.
 */

using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;

namespace strange.extensions.context.impl
{
	public class CrossContext : Context, ICrossContextCapable
	{
		private ICrossContextInjectionBinder _injectionBinder;
		private IBinder _crossContextBridge;

		/// A Binder that handles dependency injection binding and instantiation
		public ICrossContextInjectionBinder injectionBinder
		{
			get { return _injectionBinder ?? (_injectionBinder = new CrossContextInjectionBinder()); }
		    set { _injectionBinder = value; }
		}

		/// A specific instance of EventDispatcher that communicates 
		/// across multiple contexts. An event sent across this 
		/// dispatcher will be re-dispatched by the various context-wide 
		/// dispatchers. So a dispatch to other contexts is simply 
		/// 
		/// `crossContextDispatcher.Dispatch(MY_EVENT, payload)`;
		/// 
		/// Other contexts don't need to listen to the cross-context dispatcher
		/// as such, just map the necessary event to your local context
		/// dispatcher and you'll receive it.
	    protected IEventDispatcher _crossContextDispatcher;

        
		public CrossContext() : base()
		{}

		public CrossContext(object view) : base(view)
		{
		}

		public CrossContext(object view, ContextStartupFlags flags) : base(view, flags)
		{
		}

		public CrossContext(object view, bool autoMapping) : base(view, autoMapping)
		{
		}

		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			if (injectionBinder.CrossContextBinder == null)  //Only null if it could not find a parent context / firstContext
			{
				injectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
			}

			if (firstContext == this)
			{
				injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().ToSingleton().ToName(ContextKeys.CROSS_CONTEXT_DISPATCHER).CrossContext();
				injectionBinder.Bind<CrossContextBridge> ().ToSingleton ().CrossContext();
			}

		}

		protected override void instantiateCoreComponents()
		{
			base.instantiateCoreComponents();

			IInjectionBinding dispatcherBinding = injectionBinder.GetBinding<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER);

			if (dispatcherBinding != null) {
				IEventDispatcher dispatcher = injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

				if (dispatcher != null) {
					crossContextDispatcher = injectionBinder.GetInstance<IEventDispatcher> (ContextKeys.CROSS_CONTEXT_DISPATCHER) as IEventDispatcher;
					(crossContextDispatcher as ITriggerProvider).AddTriggerable (dispatcher as ITriggerable);
					(dispatcher as ITriggerProvider).AddTriggerable (crossContextBridge as ITriggerable);
				}
			}
		}

		override public IContext AddContext(IContext context)
		{
			base.AddContext(context);
			if (context is ICrossContextCapable)
			{
				AssignCrossContext((ICrossContextCapable)context);
			}
			return this;
		}

		virtual public void AssignCrossContext(ICrossContextCapable childContext)
		{
			childContext.crossContextDispatcher = crossContextDispatcher;
			childContext.injectionBinder.CrossContextBinder = injectionBinder.CrossContextBinder;
		}

		virtual public void RemoveCrossContext(ICrossContextCapable childContext)
		{
			if (childContext.crossContextDispatcher != null)
			{
				((childContext.crossContextDispatcher) as ITriggerProvider).RemoveTriggerable(childContext.GetComponent<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as ITriggerable);
				childContext.crossContextDispatcher = null;
			}
		}

		override public IContext RemoveContext(IContext context)
		{
			if (context is ICrossContextCapable)
			{
				RemoveCrossContext((ICrossContextCapable)context);
			}
			return base.RemoveContext(context);
		}

		virtual public IDispatcher crossContextDispatcher
		{
			get
			{
				return _crossContextDispatcher;
			}
			set
			{
				_crossContextDispatcher = value as IEventDispatcher;
			}
		}

		virtual public IBinder crossContextBridge
		{
			get
			{
				if (_crossContextBridge == null)
				{
					_crossContextBridge = injectionBinder.GetInstance<CrossContextBridge> () as IBinder;
				}
				return _crossContextBridge;
			}
			set
			{
				_crossContextDispatcher = value as IEventDispatcher;
			}
		}

	}
}
