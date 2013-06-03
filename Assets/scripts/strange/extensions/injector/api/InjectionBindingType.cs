using System;

namespace strange.extensions.injector.api
{
	public enum InjectionBindingType
	{
		/// The binding provides a new instance every time
		DEFAULT,

		/// The binding always provides the same instance
		SINGLETON,

		/// The binding always provides the same instance based on a provided value
		VALUE
	}
}

