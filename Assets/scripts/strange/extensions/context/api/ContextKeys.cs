using System;

namespace strange.extensions.context.api
{
	public enum ContextKeys
	{
		/// Marker for the named Injection of the Context
		CONTEXT,
		/// Marker for the named Injection of the ContextView
		CONTEXT_VIEW,
		/// Marker for the named Injection of the contextDispatcher
		CONTEXT_DISPATCHER,
		/// Marker for the named Injection of the crossContextDispatcher
		CROSS_CONTEXT_DISPATCHER
	}
}

