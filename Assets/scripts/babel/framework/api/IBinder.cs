using System;

namespace babel.framework.api
{
	public interface IBinder
	{
		//Bind a Type
		IBinding Bind<T>();

		//Bind a value
		IBinding Bind(object value);

		//Get a binding based on the provided Type
		IBinding GetBinding<T> ();

		//Get a binding based on the provided object
		IBinding GetBinding(object key);
		
		//Get a binding based on the provided Type and object name
		IBinding GetBinding<T>(object name);

		//Get a binding based on the provided object and object name
		IBinding GetBinding(object key, object name);

		//Gets a completely unformatted IBinding in whatever concrete form the Binder dictates
		IBinding GetRawBinding();

		//Remove a binding based on the provided key Type
		void Unbind<T>();

		//Remove a binding based on the provided key Type and Name
		void Unbind<T>(object name);

		//Remove a binding based on the provided key
		void Unbind (object key);

		//Remove a binding based on the provided key and name
		void Unbind (object key, object name);

		//Remove a provided binding
		void Unbind (IBinding binding);

		//Remove a select value from the given binding
		void RemoveValue (IBinding binding, object value);

		//Remove a select value from the given binding
		void RemoveKey (IBinding binding, object value);

		//Remove a select value from the given binding
		void RemoveName (IBinding binding, object value);
	}
}

