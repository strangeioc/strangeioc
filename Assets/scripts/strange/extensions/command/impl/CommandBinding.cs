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

