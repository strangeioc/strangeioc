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

/**
 * @interface strange.extensions.pool.api.IPool
 *
 * A mechanism for storing and reusing instances.
 *
 * Unlike much of the rest of Strange, the Pool is not a Binder, per se. Rather,
 * it holds onto instances created by other parts of your application for use and
 * reuse. Strange applies Pools in the CommandBinder and EventDispatcher for
 * the recycling of those instances. But you can employ Pools yourself
 * by mapping and injecting a Pool for instances you want to reuse.
 *
 * Basic instructions for injecting a Pool for use:
 * Map IPool<SomeClass> in the InjectionBinder:

		injectionBinder.Bind<IPool<MyClass>>().ToSingleton();

* Then inject like so:

		[Inject]
		public IPool<MyClass> myPool { get; set; }
 *
 * A couple of caveats for working with Pools:
 * 1. A limitation of the version of .NET currently used by Unity forbids using interfaces or abstracts in generics.
 * so you cannot map and inject IPool<IMyInterface> or IPool<MyAbstractClass>. This is a little confusing in
 * Strange, since we're used to mapping injections in exactly this fashion (e.g., injectionBinder.Bind<ISomeInterface>).
 * The reason this doesn't work for Pools has to do with setting properties, rather than the binding itself.
 * But because it will bite you, we throw an Exception if you attempt to Bind or set anything but a concrete Pool type.
 * 
 * 2. Pooling presupposes that when the instance is finished doing what it does it is cleaned up and
 * returned to the Pool. Use IPool.ReturnInstance() to mark an object as ready for reuse.
 * @see strange.extensions.pool.api.IPoolable for more on cleaning up.
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

