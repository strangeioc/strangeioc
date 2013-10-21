/// Behavior when a fixed-size pool overflows its limit.
/// By default, a fixed-size pool will throw an Exception if it is requested to
/// deliver more instances than allowed. This can be set to throw a warning or
/// to fail silently.
/// Infinitely-sized pools automatically expand in size on overflow, based on their
/// PoolInflationType.

using System;

namespace strange.framework.api
{
	public enum PoolOverflowBehavior
	{
		/// Requesting more than the fixed size will throw an exception.
		EXCEPTION,
		
		/// Requesting more than the fixed size will throw a warning.
		WARNING,

		/// Requesting more than the fixed size will return null and not throw an error.
		IGNORE
	}
}

