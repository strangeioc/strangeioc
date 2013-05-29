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

		public ISequenceBinding Once()
		{
			isOneOff = true;
			return this;
		}
		
		//Everything below this point is simply facade on Binding to ensure fluent interface
		public ISequenceBinding Bind<T>()
		{
			return Key<T> ();
		}

		public ISequenceBinding Bind(object key)
		{
			return Key (key);
		}

		new public ISequenceBinding Key<T>()
		{
			return base.Key<T> () as ISequenceBinding;
		}

		new public ISequenceBinding Key(object key)
		{
			return base.Key (key) as ISequenceBinding;
		}

		new public ISequenceBinding To<T>()
		{
			return To (typeof(T));
		}

		new public ISequenceBinding To(object o)
		{
			Type oType = o as Type;
			Type sType = typeof(ISequenceCommand);


			if (sType.IsAssignableFrom(oType) == false)
			{
				throw new SequencerException ("Attempt to bind a non SequenceCommand to a Sequence. Perhaps your command needs to extend SequenceCommand or implement ISequenCommand?", SequencerExceptionType.COMMAND_USED_IN_SEQUENCE);
			}
			
			return base.To (o) as ISequenceBinding;
		}

		new public ISequenceBinding ToName<T>()
		{
			return base.ToName<T> () as ISequenceBinding;
		}

		new public ISequenceBinding ToName(object o)
		{
			return base.ToName (o) as ISequenceBinding;
		}

		new public ISequenceBinding Named<T>()
		{
			return base.Named<T> () as ISequenceBinding;
		}

		new public ISequenceBinding Named(object o)
		{
			return base.Named (o) as ISequenceBinding;
		}
	}
}

