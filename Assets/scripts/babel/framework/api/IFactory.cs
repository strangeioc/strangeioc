using System;

namespace babel.framework.api
{
	public interface IFactory
	{
		object Get (IBinding binding);
		object Get (IBinding binding, object[] args);
	}
}

