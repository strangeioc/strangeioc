/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.extensions.sequencer.impl.SequenceBinding
 * 
 * @deprecated
 */

using System;
using strange.extensions.command.impl;
using strange.extensions.sequencer.api;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.sequencer.impl
{
	public class SequenceBinding : CommandBinding, ISequenceBinding
	{
		new public bool isOneOff{ get; set;}

		public SequenceBinding() : base()
		{
		}

		public SequenceBinding (Binder.BindingResolver resolver) : base(resolver)
		{
		}

		new public ISequenceBinding Once()
		{
			isOneOff = true;
			return this;
		}
		
		//Everything below this point is simply facade on Binding to ensure fluent interface
		new public ISequenceBinding Bind<T>()
		{
			return Key<T> ();
		}

		new public ISequenceBinding Bind(object key)
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
				throw new SequencerException ("Attempt to bind a non SequenceCommand to a Sequence. Perhaps your command needs to extend SequenceCommand or implement ISequenCommand?\n\tType: " + oType.ToString(), SequencerExceptionType.COMMAND_USED_IN_SEQUENCE);
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

