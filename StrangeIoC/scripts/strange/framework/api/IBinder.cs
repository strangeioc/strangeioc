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
 * @interface strange.framework.api.IBinder
 * 
 * Collection class for bindings.
 * 
 * Binders are a collection class (akin to ArrayList and Dictionary)
 * with the specific purpose of connecting lists of things that are
 * not necessarily related, but need some type of runtime association.
 * Binders are the core concept of the StrangeIoC framework, allowing
 * all the other functionality to exist and further functionality to
 * easily be created.
 * 
 * Think of each Binder as a collection of causes and effects, or actions
 * and reactions. If the Key action happens, it triggers the Value
 * action. So, for example, an Event may be the Key that triggers
 * instantiation of a particular class.
 * 
 * @see strange.framework.api.IBinding
 */

using System;

namespace strange.framework.api
{
	public interface IBinder
	{
		/// Bind a Binding Key to a class or interface generic
		IBinding Bind<T>();

		/// Bind a Binding Key to a value
		IBinding Bind(object value);

		/// Retrieve a binding based on the provided Type
		IBinding GetBinding<T> ();

		/// Retrieve a binding based on the provided object
		IBinding GetBinding(object key);
		
		/// Retrieve a binding based on the provided Key (generic)/Name combo
		IBinding GetBinding<T>(object name);

		/// Retrieve a binding based on the provided Key/Name combo
		IBinding GetBinding(object key, object name);

		/// Generate an unpopulated IBinding in whatever concrete form the Binder dictates
		IBinding GetRawBinding();

		/// Remove a binding based on the provided Key (generic)
		void Unbind<T>();

		/// Remove a binding based on the provided Key (generic) / Name combo
		void Unbind<T>(object name);

		/// Remove a binding based on the provided Key
		void Unbind (object key);

		/// Remove a binding based on the provided Key / Name combo
		void Unbind (object key, object name);

		/// Remove the provided binding from the Binder
		void Unbind (IBinding binding);

		/// Remove a select value from the given binding
		void RemoveValue (IBinding binding, object value);

		/// Remove a select key from the given binding
		void RemoveKey (IBinding binding, object value);

		/// Remove a select name from the given binding
		void RemoveName (IBinding binding, object value);

		/// The Binder is being removed
		/// Override this method to clean up remaining bindings
		void OnRemove();

		/// <summary>
		/// Places individual Bindings into the bindings Dictionary as part of the resolving process
		/// </summary>
		/// Note that while some Bindings may store multiple keys, each key takes a unique position in the
		/// bindings Dictionary.
		/// 
		/// Conflicts in the course of fluent binding are expected, but GetBinding
		/// will throw an error if there are any unresolved conflicts.
		void ResolveBinding(IBinding binding, object key);
	}
}

