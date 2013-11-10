/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

using System;
using strange.framework.api;

namespace strange.extensions.pool.api
{
	public interface IPool<T> : IPool
	{
		new T GetInstance ();
	}
	public interface IPool : IManagedList
	{
		/// A class that provides instances to the pool when it needs them.
		/// This can be the InjectionBinder, or any class you write that satisfies the IInstanceProvider
		/// interface.
		IInstanceProvider instanceProvider { get; set; }

		/// The object Type of the first object added to the pool.
		/// Pool objects must be of the same concrete type. This property enforces that requirement. 
		Type poolType { get; set; }

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
		/// Remove all instance references from the Pool.
		/// </summary>
		void Clean ();

		/// <summary>
		/// Returns the count of non-committed instances
		/// </summary>
		int available { get; }

		/// <summary>
		/// Gets or sets the size of the pool.
		/// </summary>
		/// <value>The pool size. '0' is a special value indicating infinite size. Infinite pools expand as necessary to accomodate requirement.</value>
		int size { get; set; }

		/// <summary>
		/// Returns the total number of instances currently managed by this pool.
		/// </summary>
		int instanceCount { get; }

		/// <summary>
		/// Gets or sets the overflow behavior of this pool.
		/// </summary>
		/// <value>A PoolOverflowBehavior value.</value>
		PoolOverflowBehavior overflowBehavior{ get; set; }

		/// <summary>
		/// Gets or sets the type of inflation for infinite-sized pools.
		/// </summary>
		/// By default, a pool doubles its InstanceCount.
		/// <value>A PoolInflationType value.</value>
		PoolInflationType inflationType{ get; set; }
	}
}

