/**
 * @interface strange.extensions.sequencer.api.ISequenceBinding
 * 
 * Defines the form of a binding for use with the Sequencer. 
 */

using System;
using strange.extensions.command.api;
using strange.extensions.sequencer.api;
using strange.framework.api;

namespace strange.extensions.sequencer.api
{
	public interface ISequenceBinding : ICommandBinding
	{
		/// Declares that the Binding is a one-off. As soon as it's satisfied, it will be unmapped.
		new ISequenceBinding Once();

		/// Get/set the property set to `true` by `Once()`
		new bool isOneOff{ get; set;}

		new ISequenceBinding Key<T>();
		new ISequenceBinding Key(object key);
		new ISequenceBinding To<T>();
		new ISequenceBinding To(object o);
		new ISequenceBinding ToName<T> ();
		new ISequenceBinding ToName (object o);
		new ISequenceBinding Named<T>();
		new ISequenceBinding Named(object o);
	}
}

