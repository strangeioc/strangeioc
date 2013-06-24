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
 * @class strange.extensions.command.impl.CommandBinding
 * 
 * The Binding for CommandBinder.
 * 
 * The only real distinction between CommandBinding and Binding
 * is the addition of `Once()`, which signals that the Binding
 * should be destroyed immediately after a single use.
 */

using System;
using strange.extensions.command.api;
using strange.framework.impl;

namespace strange.extensions.command.impl
{
	public class CommandBinding : Binding, ICommandBinding
	{
		public bool isOneOff{ get; set;}

		public bool isSequence{ get; set;}

		public CommandBinding() : base()
		{
		}

		public CommandBinding (Binder.BindingResolver resolver) : base(resolver)
		{
		}

		public ICommandBinding Once()
		{
			isOneOff = true;
			return this;
		}

		public ICommandBinding InParallel()
		{
			isSequence = false;
			return this;
		}

		public ICommandBinding InSequence()
		{
			isSequence = true;
			return this;
		}

		//Everything below this point is simply facade on Binding to ensure fluent interface
		public ICommandBinding Bind<T>()
		{
			return Key<T> ();
		}

		public ICommandBinding Bind(object key)
		{
			return Key (key);
		}

		new public ICommandBinding Key<T>()
		{
			return base.Key<T> () as ICommandBinding;
		}

		new public ICommandBinding Key(object key)
		{
			return base.Key (key) as ICommandBinding;
		}

		new public ICommandBinding To<T>()
		{
			return base.To<T> () as ICommandBinding;
		}

		new public ICommandBinding To(object o)
		{
			return base.To (o) as ICommandBinding;
		}

		new public ICommandBinding ToName<T>()
		{
			return base.ToName<T> () as ICommandBinding;
		}

		new public ICommandBinding ToName(object o)
		{
			return base.ToName (o) as ICommandBinding;
		}

		new public ICommandBinding Named<T>()
		{
			return base.Named<T> () as ICommandBinding;
		}

		new public ICommandBinding Named(object o)
		{
			return base.Named (o) as ICommandBinding;
		}
	}
}

