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

using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.framework.api;
using System.Collections.Generic;
using System;
using System.Linq;
using strange.extensions.mediation.impl;
using strange.extensions.mediation.api;
using System.Reflection;

namespace strange.extensions.context.impl
{
	public class CrossContext : Context, ICrossContextCapable
	{
		private ICrossContextInjectionBinder _injectionBinder;
		private IBinder _crossContextBridge;

		/// A Binder that handles dependency injection binding and instantiation
		public ICrossContextInjectionBinder injectionBinder
		{
			get
			{
				if (_injectionBinder == null)
				{
					_injectionBinder = new CrossContextInjectionBinder();
				}
				return _injectionBinder;
			}
			set
			{
				_injectionBinder = value;
			}
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

        //Hold a copy of the assembly so we aren't retrieving this multiple times. 
        private Assembly assembly;
		public CrossContext() : base()
		{}

		public CrossContext(object view, bool autoStartup) : base(view, autoStartup)
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
            assembly = Assembly.GetExecutingAssembly();
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

        /// <summary>
        /// Search through indicated namespaces and scan for all annotated classes.
        /// Automatically create bindings
        /// </summary>
        /// <param name="usingNamespaces">Array of namespaces. Compared using StartsWith. </param>

        protected virtual void ScanForAnnotatedClasses(string[] usingNamespaces, IMediationBinder mediationBinder)
        {

            IEnumerable<Type> types = assembly.GetExportedTypes();

            List<Type> typesInNamespaces = new List<Type>();
            int namespacesLength = usingNamespaces.Length;
            for (int ns = 0; ns < namespacesLength; ns++)
            {
                typesInNamespaces.AddRange(types.Where(t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(usingNamespaces[ns])));
            }

            foreach (Type type in typesInNamespaces)
            {

                object[] implements = type.GetCustomAttributes(typeof(DefaultImpl), true);
                object[] crossContext = type.GetCustomAttributes(typeof(CrossContextComponent), true);
                object[] implementedBy = type.GetCustomAttributes(typeof(ImplementedBy), true);
                object[] mediated = type.GetCustomAttributes(typeof(Mediated), true);
                object[] mediates = type.GetCustomAttributes(typeof(Mediates), true);


                #region Concrete and Interface Bindings

                Type bindType = null;
                Type toType = null;
                if (implements.Count() > 0)
                {
                    DefaultImpl impl = (DefaultImpl)implements.First();
                    Type[] interfaces = type.GetInterfaces();
                    int len = interfaces.Count();

                    //Confirm this type implements the type specified
                    if (impl.DefaultInterface != null)
                    {
                        if (interfaces.Contains(impl.DefaultInterface)) //Verify this Type implements the passed interface
                        {
                            bindType = impl.DefaultInterface;
                            toType = type;
                        }
                        else
                        {
                            throw new InjectionException(type.Name + " does not implement interface " + impl.DefaultInterface.Name,
                                InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT);
                        }
                    }
                    else //Concrete
                    {
                        bindType = type;
                    }
                }
                else if (implementedBy.Count() == 1)
                {
                    ImplementedBy implBy = (ImplementedBy)implementedBy.First();
                    if (implBy.DefaultType.GetInterfaces().Contains(type)) //Verify this DefaultType exists and implements the tagged interface
                    {
                        bindType = type;
                        toType = implBy.DefaultType;
                    }
                    else
                    {
                        throw new InjectionException(implBy.DefaultType.Name + " does not implement interface " + type.Name,
                            InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT);
                    }

                }


                if (bindType != null)
                {
                    IInjectionBinding binding = injectionBinder.GetBinding(bindType);
                    if (binding == null)
                    {

                        if (toType != null) //To Interface
                        {
                            binding = injectionBinder.Bind(bindType).To(toType).ToSingleton();
                        }
                        else //Concrete
                            binding = injectionBinder.Bind(type).ToSingleton();

                        if (crossContext.Count() == 1) //Bind this to the cross context injector
                        {
                            binding.CrossContext();

                        }
                    }
                    else
                    {
                        throw new InjectionException("Cannot implicitly bind to type: " + toType.Name + " because it already has an InjectionBinding.\n" +
                        " Make sure you have only one DefaultImpl or ImplementedBy pointing to this Type/Interface and are not providing this binding manually in your context.",
                            InjectionExceptionType.IMPLICIT_BINDING_ALREADY_EXISTS);
                    }
                }
                #endregion

                #region Mediations

                Type mediatorType = null;
                Type viewType = null;
                if (mediated.Count() > 0)
                {
                    viewType = type;
                    mediatorType = ((Mediated)mediated.First()).MediatorType;

                    if (mediatorType == null)
                        throw new MediationException("Cannot implicitly bind view of type: " + type.Name + " due to null MediatorType", MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);
                }
                else if (mediates.Count() > 0)
                {
                    mediatorType = type;
                    viewType = ((Mediates)mediates.First()).ViewType;

                    if (viewType == null)
                        throw new MediationException("Cannot implicitly bind Mediator of type: " + type.Name + " due to null ViewType", MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);
                }

                if (mediationBinder != null && viewType != null && mediatorType != null) //Bind this mediator!
                    mediationBinder.Bind(viewType).To(mediatorType);

                #endregion
            }
        }

	}
}
