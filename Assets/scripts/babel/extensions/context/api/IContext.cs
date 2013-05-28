/// A Context is the entry point to the binding framework.
/// ===============
/// Implement this interface to create the binding context suitable for your application.
/// 
/// In a typical Unity3D setup, a Context should be instantiated from the ContextBehaviour.

using System;
using babel.framework.api;

namespace babel.extensions.context.api
{
	public interface IContext : IBinder
	{
		IContext AddChild(IContext child);
		IContext RemoveChild(IContext child);
		IContext Start();
	}
}

