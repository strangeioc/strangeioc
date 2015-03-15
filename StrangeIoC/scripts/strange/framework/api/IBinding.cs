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
 * @interface strange.framework.api.IBinding
 * 
 * Binds a key SemiBinding to a vlaue Semibinding.
 * 
 * Bindings represent the smallest element of Strange with which most
 * developers will normally interact.
 * 
 * A Strange binding is made up of two required parts and one optional part (SemiBindings).
 * <ul>
 * <li>key - The Type or value that a client provides in order to unlock a value.</li>
 * <li>value - One or more things tied to and released by the offering of a key</li>
 * <li>name - An optional discriminator, allowing a client to differentiate between multiple keys of the same Type</li>
 * </ul>
 * 
 * The required parts are a key and a value. The key triggers the value; 
 * thus an event can be the key that triggers a callback. 
 * Or the instantiation of one class can be the key that leads to the 
 * instantiation of another class. The optional part is a name. 
 * Under some circumstances, it is useful to qualify two bindings with identical keys. 
 * Under these circumstances, the name serves as a discriminator.
 * <br />
 * <br />
 * Note that SemiBindings maintain lists, so RemoveKey, RemoveValue and RemoveName delete an entry from those lists.
 */

using System;

namespace strange.framework.api
{
	public interface IBinding
	{
		/**
		 * Tie this binding to a Type key
		 */
		IBinding Bind<T>();
		/**
		 * Tie this binding to a value key, such as a string or class instance
		 */
		IBinding Bind(object key);

		/**
		 * Set the Binding's value to a Type
		 **/
		IBinding To<T>();
		/**
		 * Set the Binding's value to a value, such as a string or class instance
		 */
		IBinding To(object o);

		/**
		 * Qualify a binding using a marker type
		 */
		IBinding ToName<T> ();

		/**
		 * Qualify a binding using a value, such as a string or class instance
		 */
		IBinding ToName (object o);

		/**
		 * Retrieve a binding if the supplied name matches, by Type
		 */
		IBinding Named<T>();

		/**
		 * Retrieve a binding if the supplied name matches, by value
		 */
		IBinding Named(object o);

		/**
		 * Remove a specific key from the binding.
		 */
		void RemoveKey (object o);

		/**
		 * Remove a specific value from the binding
		 */
		void RemoveValue (object o);

		/**
		 * Remove a name from the binding
		 */
		void RemoveName (object o);

		/// Get the binding''s key
		object key{ get; }

		/// Get the binding's name
		object name{ get; }

		/// Get the binding's value
		object value{ get; }

		/// Get or set a MANY or ONE constraint on the Key
		BindingConstraintType keyConstraint{ get; set;}

		/// Get or set a MANY or ONE constraint on the Value
		BindingConstraintType valueConstraint{ get; set;}

		//Mark a binding as weak, so that any new binding will override it
		IBinding Weak();

		//Get whether or not the binding is weak, default false
		bool isWeak { get; }

	}
}

