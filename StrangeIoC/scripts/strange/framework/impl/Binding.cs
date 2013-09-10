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
 * @class strange.framework.impl.Binding
 * 
 * A binding maintains at least two — and optionally three — SemiBindings:
 * 
 * <ul>
 * <li>key - The Type or value that a client provides in order to unlock a value.</li>
 * <li>value - One or more things tied to and released by the offering of a key</li>
 * <li>name - An optional discriminator, allowing a client to differentiate between multiple keys of the same Type</li>
 * </ul>
 * 
 * <p>Resolver</p>
 * The resolver method (type Binder.BindingResolver) is a callback passed in to resolve
 * instantiation chains.
 * 
 * @see strange.framework.api.IBinding
 * @see strange.framework.impl.Binder;
 */

using strange.framework.api;
using System;

namespace strange.framework.impl
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


		virtual public IBinding Key<T>()
		{
			return Key (typeof(T));
		}

		virtual public IBinding Key(object o)
		{
			_key.Add (o);
			return this;
		}

		virtual public IBinding To<T>()
		{
			return To (typeof(T));
		}

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

		virtual public IBinding Named<T>()
		{
			return Named (typeof(T));
		}

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