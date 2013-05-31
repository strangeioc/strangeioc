/// This is the default recommended install
/// ================
/// By extending this Context, you get
/// the entire all-singing/all-dancing version of Babel,
/// as it was shipped from the warehouse and ready for you to map
/// your dependencies.

using System;
using UnityEngine;
using babel.extensions.command.api;
using babel.extensions.command.impl;
using babel.extensions.context.api;
using babel.extensions.dispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;
using babel.extensions.injector.api;
using babel.extensions.injector.impl;
using babel.extensions.mediation.api;
using babel.extensions.mediation.impl;
using babel.extensions.sequencer.api;
using babel.extensions.sequencer.impl;
using babel.framework.api;
using babel.framework.impl;

namespace babel.extensions.context.impl
{
	public class MVCSContext : Context
	{
		new public MonoBehaviour contextView{get;set;}
		
		public IInjectionBinder injectionBinder{get;set;}
		public ICommandBinder commandBinder{get;set;}
		public IEventDispatcher dispatcher{get;set;}
		public IMediationBinder mediationBinder{get;set;}
		public ISequencer sequencer{get;set;}
		
		public MVCSContext() : base()
		{}
		
		public MVCSContext(MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{}
		
		override public IContext SetContextView(object view)
		{
			contextView = view as MonoBehaviour;
			if (contextView == null)
			{
				throw new ContextException("MVCSContext requires a ContextView of type MonoBehaviour", ContextExceptionType.NO_CONTEXT_VIEW);
			}
			return this;
		}
		
		protected override void addCoreComponents()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IInjectionBinder>().AsValue(injectionBinder);
			injectionBinder.Bind<IContext>().AsValue(this).ToName(ContextKeys.CONTEXT);
			injectionBinder.Bind<ICommandBinder>().To<CommandBinderWithEvents>().AsSingleton();
			//This binding is for local dispatchers
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>();
			//This binding is for the common system bus
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().AsSingleton().ToName(ContextKeys.CONTEXT_DISPATCHER);
			injectionBinder.Bind<IMediationBinder>().To<MediationBinder>().AsSingleton();
			injectionBinder.Bind<ISequencer>().To<SequencerWithEvents>().AsSingleton();
			if (firstContext == this)
			{
				injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().AsSingleton().ToName(ContextKeys.CROSS_CONTEXT_DISPATCHER);
			}
			else if (crossContextDispatcher != null)
			{
				injectionBinder.Bind<IEventDispatcher>().AsValue(crossContextDispatcher).ToName(ContextKeys.CROSS_CONTEXT_DISPATCHER);
			}
		}
		
		protected override void instantiateCoreComponents()
		{
			if (contextView == null)
			{
				throw new ContextException("MVCSContext requires a ContextView of type MonoBehaviour", ContextExceptionType.NO_CONTEXT_VIEW);
			}
			injectionBinder.Bind<GameObject>().AsValue(contextView.gameObject).ToName(ContextKeys.CONTEXT_VIEW);
			commandBinder = injectionBinder.GetInstance<ICommandBinder>() as ICommandBinder;
			
			dispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			mediationBinder = injectionBinder.GetInstance<IMediationBinder>() as IMediationBinder;
			sequencer = injectionBinder.GetInstance<ISequencer>() as ISequencer;
			
			(dispatcher as ITriggerProvider).AddTriggerable(commandBinder as ITriggerable);
			(dispatcher as ITriggerProvider).AddTriggerable(sequencer as ITriggerable);

			crossContextDispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CROSS_CONTEXT_DISPATCHER) as IEventDispatcher;
			(crossContextDispatcher as ITriggerProvider).AddTriggerable(dispatcher as ITriggerable);
		}
		
		protected override void postBindings()
		{
			//It's possible for views to fire their Awake before bindings. This catches any early risers and attaches their Mediators.
			mediateViewCache();
		}
		
		public override void Launch()
		{
			dispatcher.Dispatch(ContextEvent.START);
		}
		
		override public IContext AddContext(IContext context)
		{
			context.crossContextDispatcher = crossContextDispatcher;
			return this;
		}

		override public IContext RemoveContext(IContext context)
		{
			context.crossContextDispatcher = null;
			return this;
		}
		
		protected IEventDispatcher _crossContextDispatcher;
		override public IDispatcher crossContextDispatcher
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
		
		override public object GetComponent<T>()
		{
			return GetComponent<T>(null);
		}
		
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
				mediationBinder.Trigger(MediationEvent.AWAKE, view as MonoBehaviour);
			}
			else
			{
				cacheView(view as MonoBehaviour);
			}
		}
		
		override public void RemoveView(object view)
		{
			mediationBinder.Trigger(MediationEvent.DESTROYED, view as MonoBehaviour);
		}
		
		protected static ISemiBinding viewCache = new SemiBinding();
		virtual protected void cacheView(MonoBehaviour view)
		{
			if (viewCache.constraint.Equals(BindingConstraintType.ONE))
			{
				viewCache.constraint = BindingConstraintType.MANY;
			}
			viewCache.Add(view);
		}
		
		virtual protected void mediateViewCache()
		{
			if (mediationBinder == null)
				throw new ContextException("MVCSContext cannot mediate views without a mediationBinder", ContextExceptionType.NO_MEDIATION_BINDER);
			
			object[] values = viewCache.value as object[];
			if (values == null)
			{
				return;
			}
			int aa = values.Length;
			for (int a = 0; a < aa; a++)
			{
				mediationBinder.Trigger(MediationEvent.AWAKE, values[a] as MonoBehaviour);
			}
			viewCache = new SemiBinding();
		}
	}
}

