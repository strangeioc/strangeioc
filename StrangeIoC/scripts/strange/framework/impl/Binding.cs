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
 * Strange v0.7 adds Pools as an alternative form of SemiBinding. Pools can recycle groups of instances.
 * Binding implements IPool to act as a facade on any Pool SemiBinding.
 * 
 * @see strange.framework.api.IBinding;
 * @see strange.framework.api.IPool;
 * @see strange.framework.impl.Binder;
 */

using strange.framework.api;
using System;

namespace strange.framework.impl
{
	public class Binding : IBinding, IPool
	{
		public Binder.BindingResolver resolver;

		protected ISemiBinding _key;
		protected ISemiBinding _value;
		protected ISemiBinding _name;

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

		#region IBinding implementation
		public object Key
		{ 
			get
			{
				return _key.value;
			}
		}

		public object Value
		{ 
			get
			{
				return _value.value;
			}
		}

		public object Name
		{ 
			get
			{
				return (_name.value == null) ? BindingConst.NULLOID : _name.value;
			}
		}

		public Enum KeyConstraint
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

		public Enum ValueConstraint
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

		public Enum NameConstraint
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


		virtual public IBinding Bind<T>()
		{
			return Bind (typeof(T));
		}

		virtual public IBinding Bind(object o)
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

		virtual public IBinding ToPool()
		{
			ToPool (0);
			return this;
		}

		virtual public IBinding ToPool(int value)
		{
			//Cull from SemiBinding if there is one
			object existing = null;
			if (_value != null)
			{
				existing = _value.Value;
			}

			Pool pool = new Pool ();
			if (existing != null)
			{
				if (_value.Constraint.Equals(BindingConstraintType.MANY))
				{
					object[] list = _value.Value as object[];
					if (list [0] is System.Type)
						pool.PoolType = list [0] as System.Type;
					else
						pool.Add (list);
				}
				else if (existing is System.Type)
				{
					pool.PoolType = existing as System.Type;
				}
				else
				{
					pool.Add (existing);
				}
			}
			pool.Size = value;
			ValueConstraint = BindingConstraintType.POOL;
			_value = pool;
			return this;
		}

		/// [Obsolete]
		public object key
		{ 
			get
			{
				return Key;
			}
		}

		/// [Obsolete]
		public object value
		{ 
			get
			{
				return Value;
			}
		}

		/// [Obsolete]
		public object name
		{ 
			get
			{
				return Name;
			}
		}

		/// [Obsolete]
		public Enum keyConstraint
		{ 
			get
			{
				return KeyConstraint;
			}
			set
			{
				KeyConstraint = value;
			}
		}

		/// [Obsolete]
		public Enum valueConstraint
		{ 
			get
			{
				return ValueConstraint;
			}
			set
			{
				ValueConstraint = value;
			}
		}

		/// [Obsolete]
		public Enum nameConstraint
		{ 
			get
			{
				return NameConstraint;
			}
			set
			{
				NameConstraint = value;
			}
		}
		#endregion

		#region IPool implementation

		private string FAILED_FACADE = "IPool method fail in Binding not marked as Pool";

		public IManagedList Add(object value)
		{
			failIfNotPool ();
			return (_value as IPool).Add(value);
		}

		public IManagedList Add(object[] value)
		{
			failIfNotPool ();
			return (_value as IPool).Add(value);
		}

		public IManagedList Remove(object value)
		{
			failIfNotPool ();
			return (_value as IPool).Remove(value);
		}

		public IManagedList Remove(object[] value)
		{
			failIfNotPool ();
			return (_value as IPool).Remove(value);
		}

		Type IPool.PoolType
		{
			get
			{
				failIfNotPool ();
				return (_value as IPool).PoolType;
			}
			set
			{
				failIfNotPool ();
				(_value as IPool).PoolType = value;
			}
		}

		public int InstanceCount
		{
			get
			{
				failIfNotPool ();
				return (_value as IPool).InstanceCount;
			}
		}

		object IPool.GetInstance ()
		{
			failIfNotPool ();
			return (_value as IPool).GetInstance ();
		}

		void IPool.ReturnInstance (object value)
		{
			failIfNotPool ();
			(_value as IPool).ReturnInstance (value);
		}

		void IPool.Clean ()
		{
			failIfNotPool ();
			(_value as IPool).Clean ();
		}

		int IPool.Available
		{
			get
			{
				failIfNotPool ();
				return (_value as IPool).Available;
			}
		}

		int IPool.Size
		{
			get
			{
				failIfNotPool ();
				return (_value as IPool).Size;
			}
			set
			{
				failIfNotPool ();
				(_value as IPool).Size = value;
			}
		}

		PoolOverflowBehavior IPool.OverflowBehavior
		{
			get
			{
				failIfNotPool ();
				return (_value as IPool).OverflowBehavior;
			}
			set
			{
				failIfNotPool ();
				(_value as IPool).OverflowBehavior = value;
			}
		}

		PoolInflationType IPool.InflationType
		{
			get
			{
				failIfNotPool ();
				return (_value as IPool).InflationType;
			}
			set
			{
				failIfNotPool ();
				(_value as IPool).InflationType = value;
			}
		}

		#endregion

		virtual protected void failIfNotPool()
		{
			failIf (ValueConstraint.Equals(BindingConstraintType.POOL) == false, FAILED_FACADE, PoolExceptionType.FAILED_FACADE);
		}

		virtual protected void failIf(bool condition, string message, PoolExceptionType type)
		{
			if (condition)
			{
				throw new PoolException(message, type);
			}
		} 
	}
}