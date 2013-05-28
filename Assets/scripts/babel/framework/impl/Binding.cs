/**
 * Concrete binding.
 * 
 * A binding maintains at least two — and optionally three — SemiBindings:
 * - key	- The Type or value that a client provides in order to unlock a value.
 * - value	- One or more things tied to and released by the offering of a key
 * - name	- An optional discriminator, allowing a client to differentiate between multiple keys of the same Type
 * 
 * Resolver
 * A resolver method (type Binder.BindingResolver) is a callback passed in to resolve
 * instantiation chains. For example, in injectionBinder, you can declare:
 * 
 * Bind<IMyInterface>().To<MyConcreteClass>().AsSingleton();
 * 
 * The resolver fires at each stage of the daisy chain, allowing the Binder to monitor
 * and register that certain Binding actions have occurred.
 */

using babel.framework.api;
using System;

namespace babel.framework.impl
{
	public class Binding : IBinding
	{
		public Binder.BindingResolver resolver;

		protected ISemiBinding _key;
		public object key
		{ 
			get
			{
				return _key.value;
			}
		}
		protected ISemiBinding _value;
		public object value
		{ 
			get
			{
				return _value.value;
			}
		}
		protected ISemiBinding _name;
		public object name
		{ 
			get
			{
				return (_name.value == null) ? BindingConst.NULLOID : _name.value;
			}
		}

		public Enum keyConstraint
		{ 
			get
			{
				return _key.constraint;
			}
			set
			{
				_key.constraint = value;
			}
		}
		public Enum valueConstraint
		{ 
			get
			{
				return _value.constraint;
			}
			set
			{
				_value.constraint = value;
			}
		}
		public Enum nameConstraint
		{ 
			get
			{
				return _name.constraint;
			}
			set
			{
				_name.constraint = value;
			}
		}

		public Binding(Binder.BindingResolver resolver)
		{
			this.resolver = resolver;

			_key = new SemiBinding ();
			_value = new SemiBinding ();
			_name = new SemiBinding ();

			keyConstraint = BindingConstraintType.ONE;
			nameConstraint = BindingConstraintType.ONE;
			valueConstraint = BindingConstraintType.MANY;
		}

		public Binding() : this(null)
		{
		}

		//Tie this binding to a Type key
		virtual public IBinding Key<T>()
		{
			return Key (typeof(T));
		}

		//Tie this binding to a value key, such as a string
		virtual public IBinding Key(object o)
		{
			_key.Add (o);
			return this;
		}

		//Bind to a Type
		virtual public IBinding To<T>()
		{
			return To (typeof(T));
		}

		//Bind to a value
		virtual public IBinding To(object o)
		{
			_value.Add (o);
			if (resolver != null)
				resolver (this);
			return this;
		}

		virtual public IBinding ToName<T>()
		{
			return ToName (typeof(T));
		}

		virtual public IBinding ToName(object o)
		{
			object toName = (o == null) ? BindingConst.NULLOID : o;
			_name.Add(toName);
			if (resolver != null)
				resolver(this);
			return this;
		}

		//Qualify a binding using a marker type
		virtual public IBinding Named<T>()
		{
			return Named (typeof(T));
		}

		//Qualify a binding using a value, such as a string
		virtual public IBinding Named(object o)
		{
			if (_name.value == o)
				return this;
			return null;
		}

		virtual public void RemoveKey(object o)
		{
			_key.Remove (o);
		}

		virtual public void RemoveValue(object o)
		{
			_value.Remove (o);
		}

		virtual public void RemoveName(object o)
		{
			_name.Remove (o);
		}
	}
}