using System;
using babel.framework.api;

namespace babel.framework.impl
{
	public class SemiBinding : ISemiBinding
	{
		private object[] objectValue;

		public Enum constraint{ get; set;}

		public SemiBinding ()
		{
			constraint = BindingConstraintType.ONE;
		}

		public ISemiBinding Add(object o)
		{
			if (objectValue == null || (BindingConstraintType)constraint == BindingConstraintType.ONE)
			{
				objectValue = new object[1];
			}
			else
			{
				object[] tempList = objectValue;
				int len = tempList.Length;
				objectValue = new object[len + 1];
				tempList.CopyTo (objectValue, 0);
			}
			objectValue [objectValue.Length - 1] = o;

			return this;
		}

		public ISemiBinding Remove(object o)
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

		virtual public object value
		{ 
			get
			{
				if ((BindingConstraintType)constraint == BindingConstraintType.ONE)
				{
					return (objectValue == null) ? null : objectValue [0];
				}
				return objectValue;
			}
		}
	}
}

