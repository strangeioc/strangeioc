/// Command
/// =======================
/// Commands are where you place your business logic.
/// 
/// In the default strange setup, commands are mapped to TmEvents. The firing of a specific
/// event on the global event bus triggers the instantiation, injection and execution
/// of any Command(s) bound to that event.
///
/// By default, commands are cleaned up immediately on completion of the `Execute()` method.
/// For asynchronous Commands (e.g., calling a server and awaiting a response),
/// call `Retain()` at the top of your `Execute()` method, which will prevent
/// premature cleanup. But remember, having done so it is your responsipility
/// to call `Release()` once the Command is complete.

using System;

namespace strange.extensions.command.api
{
	public interface ICommand
	{
		void Execute();
		void Retain();
		void Release();
		void Dispose();

		bool retain{ get; }
		object data{ get; set;}
	}
}

