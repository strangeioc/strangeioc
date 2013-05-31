using System;

namespace strange.framework.api
{
	public interface IFactory
	{
		object Get (IBinding binding);
		object Get (IBinding binding, object[] args);
	}
}

