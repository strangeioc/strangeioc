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
	}
}

