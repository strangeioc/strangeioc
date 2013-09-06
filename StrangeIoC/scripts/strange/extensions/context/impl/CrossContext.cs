﻿using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.api;

public class CrossContext : Context, ICrossContextCapable
{
    private ICrossContextInjectionBinder _injectionBinder;
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
            injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().ToSingleton().ToName(ContextKeys.CROSS_CONTEXT_DISPATCHER);
        }
        else if (crossContextDispatcher != null)
        {
            injectionBinder.Bind<IEventDispatcher>().ToValue(crossContextDispatcher).ToName(ContextKeys.CROSS_CONTEXT_DISPATCHER);
        }
    }

    protected override void instantiateCoreComponents()
    {
        base.instantiateCoreComponents();

        IEventDispatcher dispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as IEventDispatcher;

        if (dispatcher != null)
        {
            crossContextDispatcher = injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CROSS_CONTEXT_DISPATCHER) as IEventDispatcher;
            (crossContextDispatcher as ITriggerProvider).AddTriggerable(dispatcher as ITriggerable);
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

}