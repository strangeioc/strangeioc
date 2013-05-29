using System;
using babel.framework.api;
using babel.framework.impl;
using babel.extensions.injector.api;

namespace babel.extensions.injector.impl
{
	public class InjectionBinding : Binding, IInjectionBinding
	{
		private InjectionBindingType _type = InjectionBindingType.DEFAULT;
		private bool _toInject = true;

		public InjectionBinding (Binder.BindingResolver resolver)
		{
			this.resolver = resolver;
			keyConstraint = BindingConstraintType.MANY;
			valueConstraint = BindingConstraintType.ONE;
		}

		public InjectionBindingType type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		
		public bool toInject
		{
			get
			{
				return _toInject;
			}
		}

		public IInjectionBinding AsSingleton()
		{
			type = InjectionBindingType.SINGLETON;
			if (resolver != null)
				resolver (this);
			return this;
		}

		public IInjectionBinding AsValue (object o)
		{
			Type objType = o.GetType ();

			object[] keys = key as object[];
			int aa = keys.Length;
			//Check that value is legal for the provided keys
			for (int a = 0; a < aa; a++)
			{
				object aKey = keys[a];
				Type keyType = (aKey is Type) ? aKey as Type : aKey.GetType ();
				if (keyType.IsAssignableFrom(objType) == false)
				{
					throw new InjectionException("Injection cannot bind a value that does not extend or implement the binding type.", InjectionExceptionType.ILLEGAL_BINDING_VALUE);
				}
			}
			To (o);
			type = InjectionBindingType.VALUE;
			if (resolver != null)
				resolver (this);
			return this;
		}
		
		public IInjectionBinding ToInject(bool value)
		{
			_toInject = value;
			return this;
		}

		public IInjectionBinding Bind<T>()
		{
			return Key<T> ();
		}

		public IInjectionBinding Bind(object key)
		{
			return Key (key);
		}

		//Everything below this point is simply facade on Binding to ensure fluent interface
		new public IInjectionBinding Key<T>()
		{
			return base.Key<T> () as IInjectionBinding;
		}

		new public IInjectionBinding Key(object key)
		{
			return base.Key (key) as IInjectionBinding;
		}

		new public IInjectionBinding To<T>()
		{
			return base.To<T> () as IInjectionBinding;
		}

		new public IInjectionBinding To(object o)
		{
			return base.To (o) as IInjectionBinding;
		}

		new public IInjectionBinding ToName<T>()
		{
			return base.ToName<T> () as IInjectionBinding;
		}

		new public IInjectionBinding ToName(object o)
		{
			if (o != null)
			{
				if (type == InjectionBindingType.DEFAULT)
				{
					type = InjectionBindingType.SINGLETON;
				}
			}
			return base.ToName (o) as IInjectionBinding;
		}

		new public IInjectionBinding Named<T>()
		{
			return base.Named<T> () as IInjectionBinding;
		}

		new public IInjectionBinding Named(object o)
		{
			return base.Named (o) as IInjectionBinding;
		}
	}
}

