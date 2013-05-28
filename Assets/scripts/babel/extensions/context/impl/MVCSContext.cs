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

namespace babel.extensions.context.impl
{
	public class MVCSContext : Context
	{
		new public MonoBehaviour contextView{get;set;}
		
		public IInjectionBinder injectionBinder{get;set;}
		public ICommandBinder commandBinder{get;set;}
		public IEventDispatcher dispatcher{get;set;}
		public IMediationBinder mediationBinder{get;set;}
		public IMediationBeacon mediationBeacon;
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
			injectionBinder.Bind<ICommandBinder>().To<CommandBinderWithEvents>().AsSingleton();
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().AsSingleton().ToName(ContextKeys.CONTEXT_DISPATCHER);
			injectionBinder.Bind<IMediationBinder>().To<MediationBinder>().AsSingleton();
			injectionBinder.Bind<IMediationBeacon>().To<MediationBeacon>().AsSingleton();
			injectionBinder.Bind<ISequencer>().To<SequencerWithEvents>().AsSingleton();
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
			mediationBeacon = injectionBinder.GetInstance<IMediationBeacon>() as IMediationBeacon;
			
			(dispatcher as ITriggerProvider).AddTriggerable(commandBinder as ITriggerable);
			(dispatcher as ITriggerProvider).AddTriggerable(sequencer as ITriggerable);
		}
		
		public override void Launch()
		{
			dispatcher.Dispatch(ContextEvent.START);
		}
	}
}

