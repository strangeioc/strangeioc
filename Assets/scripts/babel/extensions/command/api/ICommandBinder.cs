using System;
using babel.extensions.injector.api;
using babel.framework.api;

namespace babel.extensions.command.api
{
	public interface ICommandBinder : IBinder
	{

		/// \brief Instantiate and execute one or more Commands based on the input
		/// 
		/// \param trigger The key that unlocks the Command(s)
		void ReactTo (object trigger);
		void ReactTo (object trigger, object data);

		/// \brief Release a previously retained Command.
		/// 
		/// By default, a Command is garbage collected at the end of its Execute method. 
		/// But the Command can be retained for asynchronous calls.
		/// 
		/// \param The Command to release
		void ReleaseCommand(ICommand command);

		//Bind a Type
		new ICommandBinding Bind<T>();

		//Bind a value
		new ICommandBinding Bind(object value);
	}
}

