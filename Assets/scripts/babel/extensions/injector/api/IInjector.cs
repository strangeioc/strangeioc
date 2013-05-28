using System;
using System.Collections.Generic;
using System.Reflection;
using babel.framework.api;

namespace babel.extensions.injector.api
{
	public interface IInjector
	{
		object Instantiate (IInjectionBinding binding);
		object Inject(object target);
		IInjectionBinder binder{ get; set;}
	}
}

