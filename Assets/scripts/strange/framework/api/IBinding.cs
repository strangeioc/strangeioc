/**
 * Interface for a binding.
 * 
 * A binding maintains at least two — and optionally three — SemiBindings:
 * - key	- The Type or value that a client provides in order to unlock a value.
 * - value	- One or more things tied to and released by the offering of a key
 * - name	- An optional discriminator, allowing a client to differentiate between multiple keys of the same Type
 */

using System;

namespace strange.framework.api
{
	public interface IBinding
	{
		//Tie this binding to a Type key
		IBinding Key<T>();
		//Tie this binding to a value key, such as a string
		IBinding Key(object key);

		//Bind to a Type
		IBinding To<T>();
		//Bind to a value
		IBinding To(object o);

		//Qualify a binding using a marker type
		IBinding ToName<T> ();
		//Qualify a binding using a value, such as a string
		IBinding ToName (object o);

		//Retrieve a binding if the supplied name matches, by Type
		IBinding Named<T>();
		//Retrieve a binding if the supplied name matches, by value
		IBinding Named(object o);

		//Remove a key from the binding
		void RemoveKey (object o);
		//Remove a value from the binding
		void RemoveValue (object o);
		//Remove a name from the binding
		void RemoveName (object o);
		
		object key{ get; }
		object name{ get; }
		object value{ get; }
		Enum keyConstraint{ get; set;}
		Enum valueConstraint{ get; set;}
	}
}

