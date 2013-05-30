/// A Context is the entry point to the binding framework.
/// ===============
/// Implement this interface to create the binding context suitable for your application.
/// 
/// In a typical Unity3D setup, a Context should be instantiated from the ContextBehaviour.

using System;
using babel.framework.api;
using babel.extensions.dispatcher.api;

namespace babel.extensions.context.api
{
	public interface IContext : IBinder
	{
		//Kick the context into action
		IContext Start();
		
		//Request a component from the context (might be useful in certain cross-context situations)
		object GetComponent<T>();
		
		//Request a component from the context (might be useful in certain cross-context situations)
		object GetComponent<T>(object name);
		
		//Register a new context to this one
		IContext AddContext(IContext context);
		
		//Remove a context from this one
		IContext RemoveContext(IContext context);
		
		//Register a view with this context
		void AddView(object view);
		
		//Remove a view from this context
		void RemoveView(object view);
		
		//Set and get the shared system bus for communicating across contexts
		IDispatcher crossContextDispatcher{get;set;}
	}
}

