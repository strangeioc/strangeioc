using System;
using babel.extensions.sequencer.api;
using babel.framework.api;

namespace babel.extensions.sequencer.api
{
	public interface ISequenceBinding : IBinding
	{
		ISequenceBinding Once();
		bool isOneOff{ get; set;}
		
		///////////
		/// Below this point is facade for IBinding
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

