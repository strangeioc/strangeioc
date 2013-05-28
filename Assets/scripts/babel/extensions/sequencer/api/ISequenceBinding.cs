using System;
using babel.extensions.sequencer.api;
using babel.framework.api;

namespace babel.extensions.sequencer.api
{
	public interface ISequenceBinding : IBinding
	{
		ISequenceBinding Once();
		bool isOneOff{ get; set;}
	}
}

