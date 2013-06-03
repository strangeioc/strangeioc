/**
 * @class strange.extensions.mediation.impl.MediationException
 * 
 * An exception thrown by the Mediation system
 */

using System;
using strange.extensions.mediation.api;

namespace strange.extensions.mediation.impl
{
	public class MediationException : Exception
	{
		public MediationExceptionType type{ get; set;}

		public MediationException() : base()
		{
		}

		/// Constructs a MediationException with a message and MediationExceptionType
		public MediationException(string message, MediationExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

