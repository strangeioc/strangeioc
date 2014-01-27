using strange.extensions.context.impl;
using strange.extensions.command.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.implicitBind.api;
using strange.extensions.implicitBind.impl;
using strange.extensions.mediation.api;
using strange.extensions.sequencer.api;
using strange.extensions.injector.api;
using strange.extensions.context.api;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.mediation.impl;
using strange.extensions.sequencer.impl;
using strange.extensions.dispatcher.api;

namespace strange.unittests
{
	public class MockContext : CrossContext
	{
		public string[] ScannedPackages = new string[] {};

		/// A Binder that maps Events to Commands
		public ICommandBinder commandBinder{get;set;}

		/// A Binder that serves as the Event bus for the Context
		public IEventDispatcher dispatcher{get;set;}

		/// A Binder that maps Views to Mediators
		public IMediationBinder mediationBinder{get;set;}

		/// A Binder that maps Events to Sequences
		public ISequencer sequencer{get;set;}

		public IImplicitBinder implicitBinder { get; set; }

		public MockContext() : base() {}
		public MockContext(object view) : base(view) { }
		public MockContext(object view, bool autoStartup) : base(view, autoStartup) { }

		protected override void mapBindings()
		{
			base.mapBindings();
			implicitBinder.ScanForAnnotatedClasses(ScannedPackages);
		}

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
			injectionBinder.Bind<IImplicitBinder>().To<ImplicitBinder>().ToSingleton();
		}

		protected override void instantiateCoreComponents()
		{
			base.instantiateCoreComponents();
			commandBinder = injectionBinder.GetInstance<ICommandBinder>() as ICommandBinder;

			dispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;
			mediationBinder = injectionBinder.GetInstance<IMediationBinder>() as IMediationBinder;
			sequencer = injectionBinder.GetInstance<ISequencer>() as ISequencer;
			implicitBinder = injectionBinder.GetInstance<IImplicitBinder>() as IImplicitBinder;

			(dispatcher as ITriggerProvider).AddTriggerable(commandBinder as ITriggerable);
			(dispatcher as ITriggerProvider).AddTriggerable(sequencer as ITriggerable);

		}

		override public void AddView(object view)
		{
			mediationBinder.Trigger(MediationEvent.AWAKE, view as IView);
		}

		override public void RemoveView(object view)
		{
			mediationBinder.Trigger(MediationEvent.DESTROYED, view as IView);
		}

	}
}
