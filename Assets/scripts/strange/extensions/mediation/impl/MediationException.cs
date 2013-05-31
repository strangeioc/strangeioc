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

		public MediationException(string message, MediationExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

