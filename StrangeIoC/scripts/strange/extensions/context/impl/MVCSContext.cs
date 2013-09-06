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
 * @class strange.extensions.context.impl.MVCSContext
 * 
 * The recommended Context for getting the most out of StrangeIoC.
 * 
 * By extending this Context, you get the entire 
 * all-singing/all-dancing version of Strange, as it was shipped from the 
 * warehouse and ready for you to map your dependencies.
 * 
 * As the name suggests, MVCSContext provides structure for
 * app development using the classic <a href="http://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller">MVC (Model-View-Controller)</a>
 * design pattern, and adds 'S' (Service) for asynchronous calls outside 
 * the application. Strange is highly modular, so you needn't use
 * MVCSContext if you don't want to (you can extend Context directly)
 * but MVCS is a highly proven design strategy and by far the easiest 
 * way to get familiar with what Strange has to offer.
 * 
 * The parts:
 * <ul>
 * <li>contextView</li>
 * 
 * The GameObject at the top of your display hierarchy. Attach a subclass of
 * ContextView to a GameObject, then instantiate a subclass of MVCSContext
 * to start the app.
 * 
 * Example:

		public class MyProjectRoot : ContextView
		{
	
			void Awake()
			{
				context = new MyContext(this, true); //Extends MVCSContext
				context.Start ();
			}
		}

 * 
 * The contextView is automatically injected into all Mediators
 * and available for injection into commands like so:

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}

 * It is strong advised that the contextView NOT be injected into 
 * Views, Models or Services.
 * 
 * <li>injectionBinder</li>
 * 
 * Maps dependencies to concrete classes or values.
 * 
 * Examples:

		injectionBinder.Bind<ISpaceship>().To<TieFighter>(); //Injects a new TieFighter wherever an ISpaceship is requested
		injectionBinder.Bind<ISpaceship>().To<Starship>().ToName(Ships.ENTERPRISE); //Injects a Starship wherever an ISpaceship is requested with the Name qualifier Enterprise
		injectionBinder.Bind<ITool>().To<SonicScrewdriver>().ToSingleton(); //Injects SonicScrewdriver as a Singleton wherever an ITool is requested
		injectionBinder.Bind<IMoonbase>().ToValue(new Alpha()); //Injects the provided instance wherever IMoonbase is requested
		injectionBinder.Bind<ISpaceship>().Bind<ITimeShip>.To<Tardis>(); //Injects a new Tardis wherever EITHER ISpaceship or ITimeship is requested.

 * `injectionBinder` is automatically injected into all commands and may
 * be injected elsewhere with:

		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}

 * <li>dispatcher</li>
 * 
 * The event bus shared across the context. Informs listeners
 * and triggers commands.
 * 
 * `dispatcher` is injected into all EventMediators, EventCommands
 * and EventSequenceCommands, and may be injected elsewhere with:

 		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

 * For examples, see IEventDispatcher. Generally you don't map the dispatcher's
 * events to methods inside the Context. Rather, you map Commands and Sequences.
 * Read on!
 * 
 * <li>crossContextDispatcher</li>
 * 
 * A second event bus for sending events between contexts. It
 * should only be accessed from Commands or SequenceCommands,
 * into which it may be injected by declaring the dependency:

		[Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher{ get; set;}

 * 
 * <li>commandBinder</li>
 * 
 * Maps events that result in the creation and execution of Commands.
 * Events from dispatcher can be used to trigger Commands.
 * 
 * `commandBinder` is automatically injected into all Commands.
 * 
 * Examples:

		commandBinder.Bind(GameEvent.MISSILE_HIT).To<MissileHitCommand>(); //MissileHitCommand fires whenever MISSILE_HIT is dispatched
		commandBinder.Bind(GameEvent.MISSILE_HIT).To<IncrementScoreCommand>().To<UpdateServerCommand>(); //Both Commands fire
		commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once(); //StartCommand fires when START fires, then unmaps itself

 * 
 * <li>sequencer</li>
 * 
 * Maps events that result in the creation and execution of Sequences,
 * which are just like Commands, except they run sequentially, rather than
 * in parallel.
 * 
 * 'sequencer' is automatically injected into all SequenceCommands.
 * 
 * In the following example, `TestMissileHitCommand` runs logic to determine
 * whether the missile hit is valid. If it's not, it may call `BreakSeqeunce()`.
 * so neither of the other Commands will fire.

		sequencer.Bind(GameEvent.MISSILE_HIT).To<TestMissileHitCommand>().To<IncrementScoreCommand>().To<UpdateServerCommand>();

 * 
 * <li>mediationBinder</li>
 * 
 * Maps Views to Mediators in order to insultate Views from direct 
 * linkage to the application.
 * 
 * MediationBinder isn't automatically injected anywhere. It is
 * possible, however, that you might want to change mediation bindings
 * at runtime. This might prove difficult as a practical matter, but
 * if you want to experiment, feel free to inject `mediationBinder`
 * into Commands or SequenceCommands like so:

		[Inject]
		IMediationBinder mediationBinder{get;set;}

 * 
 * Example:

		mediationBinder.Bind<RobotView>().To<RobotMediator>();
 *	<ul>
 * 
 * 
 */

using UnityEngine;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using strange.extensions.sequencer.api;
using strange.extensions.sequencer.impl;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.context.impl
{
	public class MVCSContext : CrossContext
	{
		/// A Binder that maps Events to Commands
		public ICommandBinder commandBinder{get;set;}

		/// A Binder that serves as the Event bus for the Context
		public IEventDispatcher dispatcher{get;set;}

		/// A Binder that maps Views to Mediators
		public IMediationBinder mediationBinder{get;set;}

		/// A Binder that maps Events to Sequences
		public ISequencer sequencer{get;set;}


		/// A list of Views Awake before the Context is fully set up.
		protected static ISemiBinding viewCache = new SemiBinding();
		
		public MVCSContext() : base()
		{}
		
		public MVCSContext(MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
        }
		
		override public IContext SetContextView(object view)
		{
			contextView = (view as MonoBehaviour).gameObject;
			if (contextView == null)
			{
				throw new ContextException("MVCSContext requires a ContextView of type MonoBehaviour", ContextExceptionType.NO_CONTEXT_VIEW);
			}
			return this;
		}

		/// Map the relationships between the Binders.
		/// Although you can override this method, it is recommended
		/// that you provide all your application bindings in `mapBindings()`.
		protected override void addCoreComponents()
		{
            base.addCoreComponents();
			injectionBinder.Bind<IInjectionBinder>().ToValue(injectionBinder);
			injectionBinder.Bind<IContext>().ToValue(this).ToName(ContextKeys.CONTEXT);
			injectionBinder.Bind<ICommandBinder>().To<EventCommandBinder>().ToSingleton();
			//This binding is for local dispatchers
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>();
			//This binding is for the common system bus
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().ToSingleton().ToName(ContextKeys.CONTEXT_DISPATCHER);
			injectionBinder.Bind<IMediationBinder>().To<MediationBinder>().ToSingleton();
			injectionBinder.Bind<ISequencer>().To<EventSequencer>().ToSingleton();
		}
		
		protected override void instantiateCoreComponents()
		{
            base.instantiateCoreComponents();
			if (contextView == null)
			{
				throw new ContextException("MVCSContext requires a ContextView of type MonoBehaviour", ContextExceptionType.NO_CONTEXT_VIEW);
			}
			injectionBinder.Bind<GameObject>().ToValue(contextView).ToName(ContextKeys.CONTEXT_VIEW);
			commandBinder = injectionBinder.GetInstance<ICommandBinder>() as ICommandBinder;
			
			dispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			mediationBinder = injectionBinder.GetInstance<IMediationBinder>() as IMediationBinder;
			sequencer = injectionBinder.GetInstance<ISequencer>() as ISequencer;
			
			(dispatcher as ITriggerProvider).AddTriggerable(commandBinder as ITriggerable);
			(dispatcher as ITriggerProvider).AddTriggerable(sequencer as ITriggerable);
		}
		
		protected override void postBindings()
		{
			//It's possible for views to fire their Awake before bindings. This catches any early risers and attaches their Mediators.
			mediateViewCache();
		}

		/// Fires ContextEvent.START
		/// Whatever Command/Sequence you want to happen first should 
		/// be mapped to this event.
		public override void Launch()
		{
			dispatcher.Dispatch(ContextEvent.START);
		}
		
		/// Gets an instance of the provided generic type.
		/// Always bear in mind that doing this risks adding
		/// dependencies that must be cleaned up when Contexts
		/// are removed.
		override public object GetComponent<T>()
		{
			return GetComponent<T>(null);
		}

		/// Gets an instance of the provided generic type and name from the InjectionBinder
		/// Always bear in mind that doing this risks adding
		/// dependencies that must be cleaned up when Contexts
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
				cacheView(view as MonoBehaviour);
			}
		}
		
		override public void RemoveView(object view)
		{
			mediationBinder.Trigger(MediationEvent.DESTROYED, view as IView);
		}

		/// Caches early-riser Views.
		/// 
		/// If a View is on stage at startup, it's possible for that
		/// View to be Awake before this Context has finished initing.
		/// `cacheView()` maintains a list of such 'early-risers'
		/// until the Context is ready to mediate them.
		virtual protected void cacheView(MonoBehaviour view)
		{
			if (viewCache.constraint.Equals(BindingConstraintType.ONE))
			{
				viewCache.constraint = BindingConstraintType.MANY;
			}
			viewCache.Add(view);
		}

		/// Provide mediation for early-riser Views
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
				mediationBinder.Trigger(MediationEvent.AWAKE, values[a] as IView);
			}
			viewCache = new SemiBinding();
		}

        public override void OnRemove()
        {
            base.OnRemove();
            commandBinder.OnRemove();
        }

    }
}

