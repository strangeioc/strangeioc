using System;

namespace strange.extensions.injector.api
{
	public enum InjectionExceptionType
	{
		CIRCULAR_DEPENDENCY,
		ILLEGAL_BINDING_VALUE,
		NO_BINDER,
		NO_FACTORY,
		NULL_BINDING,
		NULL_CONSTRUCTOR,
		NULL_INJECTION_POINT,
		NULL_TARGET,
		NULL_VALUE_INJECTION
	}
}

