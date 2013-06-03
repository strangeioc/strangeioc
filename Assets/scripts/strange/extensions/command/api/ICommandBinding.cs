/**
 * @interface strange.extensions.command.api.ICommandBinding
 * 
 * Defines the form of a Binding for use with the CommandBinder. 
 * 
 * The only real distinction between CommandBinding and Binding
 * is the addition of `Once()`, which signals that the Binding
 * should be destroyed immediately after a single use.
 */

using System;
using strange.framework.api;

namespace strange.extensions.command.api
{
	public interface ICommandBinding : IBinding
	{
		/// Declares that the Binding is a one-off. As soon as it's satisfied, it will be unmapped.
		ICommandBinding Once();

		/// Get/set the property set to `true` by `Once()`
		bool isOneOff{ get; set;}

		new ICommandBinding Key<T>();
		new ICommandBinding Key(object key);
		new ICommandBinding To<T>();
		new ICommandBinding To(object o);
		new ICommandBinding ToName<T> ();
		new ICommandBinding ToName (object o);
		new ICommandBinding Named<T>();
		new ICommandBinding Named(object o);
	}
}

