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
 * @class strange.framework.impl.SemiBinding
 * 
 * A managed list of values.
 * 
 * @see strange.framework.api.ISemiBinding
 */

using System;
using strange.framework.api;

namespace strange.framework.impl
{
	public class SemiBinding : ISemiBinding
	{
		protected object[] objectValue;

		public Enum constraint{ get; set;}
		public bool uniqueValues{ get; set;}

		public SemiBinding ()
		{
			constraint = BindingConstraintType.ONE;
			uniqueValues = true;
		}

		#region IManagedList implementation

		public IManagedList Add(object o)
		{
			if (objectValue == null || (BindingConstraintType)constraint == BindingConstraintType.ONE)
			{
				objectValue = new object[1];
			}
			else
			{
				if (uniqueValues)
				{
					int aa = objectValue.Length;
					for (int a = 0; a < aa; a++)
					{
						object val = objectValue[a];
						if (val.Equals(o))
						{
							return this;
						}
					}
				}
				
				object[] tempList = objectValue;
				int len = tempList.Length;
				objectValue = new object[len + 1];
				tempList.CopyTo (objectValue, 0);
			}
			objectValue [objectValue.Length - 1] = o;

			return this;
		}

		public IManagedList Add(object[] list)
		{
			foreach (object item in list)
				Add (item);

			return this;
		}

		public IManagedList Remove(object o)
		{
			if (o.Equals(objectValue) || objectValue == null)
			{
				objectValue = null;
				return this;
			}
			int aa = objectValue.Length;
			for(int a = 0; a < aa; a++)
			{
				object currVal = objectValue [a];
				if (o.Equals(currVal))
				{
					spliceValueAt (a);
					return this;
				}
			}
			return this;
		}

		public IManagedList Remove(object[] list)
		{
			foreach (object item in list)
				Remove (item);

			return this;
		}
		virtual public object value
		{ 
			get
			{
				if (constraint.Equals(BindingConstraintType.ONE))
				{
					return (objectValue == null) ? null : objectValue [0];
				}
				return objectValue;
			}
		}

		#endregion

		/// Remove the value at index splicePos
		protected void spliceValueAt(int splicePos)
		{
			object[] newList = new object[objectValue.Length - 1];
			int mod = 0;
			int aa = objectValue.Length;
			for(int a = 0; a < aa; a++)
			{
				if (a == splicePos)
				{
					mod = -1;
					continue;
				}
				newList [a + mod] = objectValue [a];
			}
			objectValue = (newList.Length == 0) ? null : newList;
		}
	}
}

