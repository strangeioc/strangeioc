/**
 * @interface strange.extensions.command.api.ICommand
 * 
 * Interface for Commands, which is where you place your business logic.
 * 
 * In the default StrangeIoC setup, commands are mapped to TmEvents. 
 * The firing of a specific event on the global event bus triggers 
 * the instantiation, injection and execution of any Command(s) bound to that event.
 * 
 * By default, commands are cleaned up immediately on completion of the `Execute()` method.
 * For asynchronous Commands (e.g., calling a service and awaiting a response),
 * call `Retain()` at the top of your `Execute()` method, which will prevent
 * premature cleanup. But remember, having done so it is your responsipility
 * to call `Release()` once the Command is complete.
 */ 

using System;

namespace strange.extensions.command.api
{
	public interface ICommand
	{
		/// Override this! `Execute()` is where you place the logic for your Command.
		void Execute();

		/// Keeps the Command in memory. Use only in conjunction with `Release()`
		void Retain();

		/// Allows a previous Retained Command to be disposed.
		void Release();

		/// The property set by `Retain` and `Release` to indicate whether the Command should be cleaned up on completion of the `Execute()` method. 
		bool retain{ get; }

		/// A payload injected into the Command. Most commonly, this a TmEvent.
		object data{ get; set;}
	}
}

