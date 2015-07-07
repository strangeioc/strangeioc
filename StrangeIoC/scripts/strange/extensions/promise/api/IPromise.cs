/*
 * Copyright 2015 StrangeIoC
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
 * @interface strange.extensions.promise.api.IPromise
 *
 * Promises are an elegant pattern for handling asynchronous data, as well as data 
 * that may or may not be available at the moment a request is made. The most obvious
 * asynch case would be a web request, which takes time to roundtrip. Cases when data
 * may or may not be available may include bootstrapping, when certain data may become
 * accessible in unpredictable order. By applying Promises in these cases, we can grab
 * the data immediately if it's available, or eventually whenever it becomes available.
 *
 * Promises implement the following callback structure. Each method in the chain is a callback,
 * and all callbacks are optional.
 *
 * promise
 *	.Then(NoArgCallback)
 *	.Progress(ProgressCallback)
 *	.Fail(FailCallback)
 *	.Finally(FinallyCallback)
 *
 * Promises in Strange use the Action Class as the underlying mechanism for type safety.
 * Unity's C# implementation currently allows up to FOUR parameters in an Action, therefore
 * PROMISES ARE LIMITED TO FOUR PARAMETERS. If you require more than four, consider
 * creating a value object to hold additional values.
 *
 * Promises are inspired by Kris Kowal's Q.
 */

using System;
using strange.extensions.promise.impl;

namespace strange.extensions.promise.api
{
	public interface IBasePromise
	{
		/// <summary>
		/// Handle a callback as the Promise updates.
		/// </summary>
		/// <param name="listener">The callback.</param>
		IBasePromise Progress(Action<float> listener);
		/// <summary>
		/// Handle a callback if the Promise fails to deliver.
		/// </summary>
		/// The callback must handle an Exception
		/// <param name="listener">The callback.</param>
		IBasePromise Fail(Action<Exception> listener);
		/// <summary>
		/// Trigger a callback after the Promise has concluded its business.
		/// </summary>
		/// <param name="listener">The callback.</param>
		IBasePromise Finally(Action listener);

		/// <summary>
		/// Report an Exception thrown in the course of attempting to fulfill the Promise.
		/// </summary>
		/// If there's an OnFail callback, fire it.
		/// <param name="ex">The exception.</param>
		void ReportFail(Exception ex);
		/// <summary>
		/// Report progress in the course of attempting to fulfill the Promise.
		/// </summary>
		/// <param name="progress">A value representing the percentage of completion. By convention, this 
		/// value is either a float 0-1 or a float 0-100, but the specific implementer is a matter for the
		/// designer of the Promise.</param>
		void ReportProgress(float progress);

		/// <summary>
		/// Removes all listeners.
		/// </summary>
		void RemoveAllListeners();
		/// <summary>
		/// Removes the progress listeners.
		/// </summary>
		void RemoveProgressListeners();
		/// <summary>
		/// Removes the fail listeners.
		/// </summary>
		void RemoveFailListeners();

		/// <summary>
		/// Implement in concrete class. The number of clients for this Promise.
		/// </summary>
		/// <returns>The count.</returns>
		int ListenerCount();

		BasePromise.PromiseState State { get; }
	}
	public interface IPromise : IBasePromise
	{
		IPromise Then(Action action);
		void Dispatch();
		void RemoveListener(Action action);
	}

	public interface IPromise<T> : IBasePromise
	{
		IPromise<T> Then(Action<T> action);
		void Dispatch(T t);
		void RemoveListener(Action<T> action);
	}

	public interface IPromise<T, U> : IBasePromise
	{
		IPromise<T, U> Then(Action<T, U> action);
		void Dispatch(T t, U u);
		void RemoveListener(Action<T, U> action);
	}

	public interface IPromise<T, U, V> : IBasePromise
	{
		IPromise<T, U, V> Then(Action<T, U, V> action);
		void Dispatch(T t, U u, V v);
		void RemoveListener(Action<T, U, V> action);
	}

	public interface IPromise<T, U, V, W> : IBasePromise
	{
		IPromise<T, U, V, W> Then(Action<T, U, V, W> action);
		void Dispatch(T t, U u, V v, W w);
		void RemoveListener(Action<T, U, V, W> action);
	}


}
