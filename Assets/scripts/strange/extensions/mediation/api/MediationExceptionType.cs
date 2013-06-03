using System;

namespace strange.extensions.mediation.api
{
	public enum MediationExceptionType
	{
		/// Exception raised when a View can't locate a Context.
		/// Views contact the Context by "bubbling" their existence up 
		/// the display chain (recursively using transform.parent).
		/// If a View reaches the top of that chain without locating
		/// a Context, it will raise this Exception to warn you.
		/// 
		/// Note: to avoid infinite looping, there is a bubbling limit of 100
		/// layers. If your View needs to be more than 100 transforms deep, 
		/// that might signal a design problem.
		NO_CONTEXT
	}
}

