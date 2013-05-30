using System;
using babel.extensions.mediation.api;

namespace babel.extensions.mediation.impl
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

