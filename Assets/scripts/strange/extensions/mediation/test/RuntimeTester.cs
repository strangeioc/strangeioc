using System;
using UnityEngine;

namespace strange.extensions.mediation.test
{
	public class RuntimeTester : MonoBehaviour
	{
		public RuntimeTester ()
		{
		}
		
		protected void failIf(bool condition, string message)
		{
			if (condition)
				throw new Exception(message);
		}
	}
}

