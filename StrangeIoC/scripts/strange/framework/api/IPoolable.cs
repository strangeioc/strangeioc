/// <summary>
/// Interface for any item that can be pooled.
/// </summary>

using System;

namespace strange.framework.api
{
	public interface IPoolable
	{
		/// <summary>
		/// Release this instance back to the pool.
		/// </summary>
		/// Release methods should clean up the instance sufficiently to remove prior state.
		void Release ();
	}
}

