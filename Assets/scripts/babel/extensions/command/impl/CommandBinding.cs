using System;
using babel.extensions.command.api;
using babel.framework.impl;

namespace babel.extensions.command.impl
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

