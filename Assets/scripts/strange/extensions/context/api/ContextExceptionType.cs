using System;

namespace strange.extensions.context.api
{
	public enum ContextExceptionType
	{
		/// MVCSContext requires a root ContextView
		NO_CONTEXT_VIEW,
		/// MVCSContext requires a mediationBinder
		NO_MEDIATION_BINDER
	}
}

