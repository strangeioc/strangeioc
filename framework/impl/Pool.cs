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

using System;
using System.Collections;
using System.Collections.Generic;
using strange.framework.api;

namespace strange.framework.impl
{
	public class Pool : ISemiBinding, IPool, IPoolable
	{
		/// The object Type of the first object added to the pool.
		/// Pool objects must be of the same concrete type. This property enforces that requirement. 
		protected Type poolType;

		/// Stack of instances still in the Pool.
		protected Stack instancesAvailable = new Stack ();

		/// A HashSet of the objects checked out of the Pool.
		protected HashSet<object> instancesInUse = new HashSet<object> ();

		public Pool () : base()
		{
			Size = 0;
			constraint = BindingConstraintType.POOL;
			uniqueValues = true;

			OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
			InflationType = PoolInflationType.INCREMENT;
		}

		#region IManagedList implementation

		virtual public IManagedList Add (object value)
		{
			if (poolType == null)
			{
				poolType = value.GetType ();
			}
			else
			{
				failIf(value.GetType () != poolType, "Pool Type mismatch. Pools must consist of a common concrete type.\n\t\tPool type: " + poolType.ToString() + "\n\t\tMismatch type: " + value.GetType ().ToString(), PoolExceptionType.TYPE_MISMATCH);
			}
			instancesAvailable.Push (value);
			return this;
		}

		virtual public IManagedList Add (object[] list)
		{
			foreach (object item in list)
				Add (item);

			return this;
		}

		virtual public IManagedList Remove (object value)
		{
			removeInstance (value);
			return this;
		}

		virtual public IManagedList Remove (object[] list)
		{
			foreach (object item in list)
				Remove (item);

			return this;
		}

		virtual public object Value 
		{
			get 
			{
				return GetInstance ();
			}
		}

		/// [Obsolete"Strange migration to conform to C# guidelines. Removing camelCased publics"]
		virtual public object value
		{
			get { return Value; }
		}
		#endregion

		#region ISemiBinding region
		virtual public bool UniqueValues{get;set;}
		virtual public Enum Constraint { get; set; }

		/// [Obsolete"Strange migration to conform to C# guidelines. Removing camelCased publics"]
		virtual public bool uniqueValues
		{
			get
			{
				return UniqueValues;
			}
			set
			{
				UniqueValues = value;
			}
		}

		/// [Obsolete"Strange migration to conform to C# guidelines. Removing camelCased publics"]
		virtual public Enum constraint{
			get
			{
				return Constraint;
			}
			set
			{
				Constraint = value;
			}
		}

		#endregion

		#region IPool implementation

		virtual public object GetInstance ()
		{
			// Is an instance available?
			if (instancesAvailable.Count > 0)
			{
				object retv = instancesAvailable.Pop ();
				instancesInUse.Add (retv);
				return retv;
			}

			failIf(OverflowBehavior == PoolOverflowBehavior.EXCEPTION,
			       "A pool has overflowed its limit.\n\t\tPool type: " + poolType,
			       PoolExceptionType.OVERFLOW);

			if (OverflowBehavior == PoolOverflowBehavior.WARNING)
			{
				Console.WriteLine ("WARNING: A pool has overflowed its limit.\n\t\tPool type: " + poolType, PoolExceptionType.OVERFLOW);
			}

			return null;
		}

		virtual public void ReturnInstance (object value)
		{
			if (instancesInUse.Contains (value))
			{
				if (value is IPoolable)
				{
					(value as IPoolable).Release ();
				}
				instancesInUse.Remove (value);
				instancesAvailable.Push (value);
			}
		}

		virtual public void Clean()
		{
			instancesAvailable.Clear ();
			instancesInUse = new HashSet<object> ();
		}

		virtual public int Available
		{
			get
			{
				return instancesAvailable.Count;
			}
		}

		virtual public int Size { get; set; }

		virtual public PoolOverflowBehavior OverflowBehavior { get; set; }

		virtual public PoolInflationType InflationType { get; set; }

		#endregion

		#region IPoolable implementation

		public void Release ()
		{
			Clean ();
			Size = 0;
		}

		#endregion

		/// <summary>
		/// Permanently removes an instance from the Pool
		/// </summary>
		/// In the event that the removed Instance is in use, it is removed from instancesInUse.
		/// Otherwise, it is presumed inactive, and the next available object is popped from
		/// instancesAvailable.
		/// <param name="value">An instance to remove permanently from the Pool.</param>
		virtual protected void removeInstance(object value)
		{
			failIf (value.GetType() != poolType, "Attempt to remove a instance from a pool that is of the wrong Type:\n\t\tPool type: " + poolType.ToString() + "\n\t\tInstance type: " + value.GetType().ToString(), PoolExceptionType.TYPE_MISMATCH);
			if (instancesInUse.Contains(value))
			{
				instancesInUse.Remove (value);
			}
			else
			{
				instancesAvailable.Pop ();
			}
		}

		protected void failIf(bool condition, string message, PoolExceptionType type)
		{
			if (condition)
			{
				throw new PoolException(message, type);
			}
		}
	}
}

