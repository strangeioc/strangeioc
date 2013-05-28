using System;
using babel.framework.api;

namespace babel.extensions.command.api
{
	public interface ICommandBinding : IBinding
	{
		ICommandBinding Once();
		bool isOneOff{ get; set;}

		///////////
		/// Below this point is facade for IBinding
		new ICommandBinding Key<T>();
		new ICommandBinding Key(object key);
		new ICommandBinding To<T>();
		new ICommandBinding To(object o);
		new ICommandBinding ToName<T> ();
		new ICommandBinding ToName (object o);
		new ICommandBinding Named<T>();
		new ICommandBinding Named(object o);
	}
}

