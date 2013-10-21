using System;
using strange.framework.api;

namespace strange.framework.api
{
	public interface IPool
	{
		/// <summary>
		/// Gets an instance from the pool if one is available.
		/// </summary>
		/// <returns>The instance.</returns>
		object GetInstance();

		/// <summary>
		/// Returns an instance to the pool.
		/// </summary>
		/// If the instance being released implements IPoolable, the Release() method will be called.
		/// <param name="value">The instance to be return to the pool.</param>
		void ReturnInstance (object value);

		/// <summary>
		/// Gets or sets the size of the pool.
		/// </summary>
		/// <value>The pool size. '0' is a special value indicating infinite size. Infinite pools expand as necessary to accomodate requirement.</value>
		int Size { get; set; }

		/// <summary>
		/// Gets or sets the overflow behavior of this pool.
		/// </summary>
		/// <value>A PoolOverflowBehavior value.</value>
		PoolOverflowBehavior OverflowBehavior{ get; set; }

		/// <summary>
		/// Gets or sets the type of inflation for infinite-sized pools.
		/// </summary>
		/// <value>A PoolInflationType value.</value>
		PoolInflationType InflationType{ get; set; }
	}
}

