using System;
using babel.extensions.sequencer.api;
using babel.framework.api;
using babel.framework.impl;

namespace babel.extensions.sequencer.impl
{
	public class SequenceBinding : Binding, ISequenceBinding
	{
		public bool isOneOff{ get; set;}

		public SequenceBinding() : base()
		{
		}

		public SequenceBinding (Binder.BindingResolver resolver) : base(resolver)
		{
		}

		public override IBinding To (object o)
		{
			Type oType = o as Type;
			Type sType = typeof(ISequenceCommand);


			if (sType.IsAssignableFrom(oType) == false)
			{
				throw new SequencerException ("Attempt to bind a non SequenceCommand to a Sequence. Perhaps your command needs to extend SequenceCommand or implement ISequenCommand?", SequencerExceptionType.COMMAND_USED_IN_SEQUENCE);
			}
			return base.To (o);
		}

		public ISequenceBinding Once()
		{
			isOneOff = true;
			return this;
		}
	}
}

