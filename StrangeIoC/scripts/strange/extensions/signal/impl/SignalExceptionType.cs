
namespace strange.extensions.signal.impl
{
	public enum SignalExceptionType
	{

        ///Attempting to bind more than one value of the same type to a command
        COMMAND_VALUE_CONFLICT,
        COMMAND_VALUE_NOT_FOUND,
        COMMAND_NULL_INJECTION,
	}
}
