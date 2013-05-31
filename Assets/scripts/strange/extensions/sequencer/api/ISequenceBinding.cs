using System;
using strange.extensions.sequencer.api;
using strange.framework.api;

namespace strange.extensions.sequencer.api
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

