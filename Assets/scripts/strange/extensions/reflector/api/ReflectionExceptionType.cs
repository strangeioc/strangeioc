using System;

namespace strange.extensions.reflector.api
{
	public enum ReflectionExceptionType
	{
		/// The reflector requires a constructor, which Interfaces don't provide.
		CANNOT_REFLECT_INTERFACE
	}
}

