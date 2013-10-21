using System;
using strange.framework.api;

namespace strange.framework.impl
{
	public class Pool : SemiBinding, IPool
	{
		/// Bitwise abacus to track items retained by/released from the pool.
		public int tracker = 0;

		public Pool () : base()
		{
			constraint = BindingConstraintType.MANY;
			Size = 1;
			OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
			InflationType = PoolInflationType.INCREMENT;
		}

		#region IPool implementation

		virtual public object GetInstance ()
		{
			if (Size == 1)
			{
				return value;
			}

			int nextAvailable = getEmplySlot ();




			if (upperBound < Size)
			{
				return objectValue [upperBound];
			}


			return null;
			// Is there an available instance?
			//return it
			// Is pool infinite?
			//Inflate
			//return a new instance
			// If not infinite
			//throw warning/error or return null
		}

		virtual public void ReturnInstance (object value)
		{
			if (value is IPoolable)
			{
				(value as IPoolable).Release ();
			}
			// Mark the instance as free

		}

		public int Size { get; set; }

		public PoolOverflowBehavior OverflowBehavior { get; set; }

		public PoolInflationType InflationType { get; set; }

		#endregion

		#region abacus tracker functions
		//
		public int getBitwisePosition(int value)
		{
			return (1 << value) >> 1;
		}

		/// Returns the next unfilled slot
		public int getEmptySlot()
		{
			return (~tracker & (tracker+1)) >> 1;
		}

		/// Mark the indicated slot as occupied
		public void fillSlot(int value)
		{
			setSlot (1, value);
		}

		/// Mark the indicated slot as unoccupied
		public void clearSlot(int value)
		{
			setSlot (0, value);
		}

		protected void setSlot(int toSet, int value)
		{
			tracker = (tracker & ~(1 << value)) | (toSet << value);
		}
		#endregion
	}
}

