using strange.extensions.context.impl;
using strange.extensions.command.impl;
using UnityEngine;
using strange.framework.impl;
using System.Diagnostics;
using Mobile.Utils;

/** PCYR: NestedContext written to allow us to inherit bindings from
 * other contexts like how RobotLegs does it.  I actually don't know
 * if RL actually does it this way, but the net effect seems to be the same.
 */
public class NestedContext : MVCSContext
{
	private MVCSContext parentContext;
	
	public NestedContext() : base()
	{}
	
	public NestedContext(MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
	{
	}
	
	public void MakeChildOf(MVCSContext parent)
	{	
#if STRANGE_DEBUG
		Logger.Instance.Log("NestedContext : Making ("+((Binder) injectionBinder.injector.binder).id+") a child of ("+((Binder) parent.injectionBinder.injector.binder).id+").", Logger.FilterTags.Info);
#endif
		
		Assert.assert(null == parentContext, "MakeChildOf can only be called once!");
		Assert.assert(null != parent, "parent must not be null!");
		parentContext = parent;
		
		Binder injBinder = injectionBinder as Binder;
		injBinder.SetParent(parent.injectionBinder as Binder);
		
		// Tried this during great nodes debug thing.  Not sure if it is necessary.
//		Binder refBinder = injectionBinder.injector.reflector as Binder;
//		refBinder.SetParent(parent.injectionBinder.injector.reflector as Binder);
		
		commandBinder.SetParent(parent.commandBinder);
		
		Binder dispatchBinder = (Binder) dispatcher;
		dispatchBinder.SetParent(parent.dispatcher as Binder);
	}
}