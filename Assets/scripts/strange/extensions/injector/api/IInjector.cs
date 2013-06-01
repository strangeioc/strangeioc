using System;
using System.Collections.Generic;
using strange.extensions.reflector.api;
using strange.framework.api;

namespace strange.extensions.injector.api
{
	public interface IInjector
	{
		object Instantiate (IInjectionBinding binding);
		object Inject(object target);
		IInjectionBinder binder{ get; set;}
		IReflectionBinder reflector{ get; set;}
	}
}

