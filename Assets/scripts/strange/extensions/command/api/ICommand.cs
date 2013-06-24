/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @interface strange.extensions.command.api.ICommand
 * 
 * Interface for Commands, which is where you place your business logic.
 * 
 * In the default StrangeIoC setup, commands are mapped to IEvents. 
 * The firing of a specific event on the global event bus triggers 
 * the instantiation, injection and execution of any Command(s) bound to that event.
 * 
 * By default, commands are cleaned up immediately on completion of the `Execute()` method.
 * For asynchronous Commands (e.g., calling a service and awaiting a response),
 * call `Retain()` at the top of your `Execute()` method, which will prevent
 * premature cleanup. But remember, having done so it is your responsipility
 * to call `Release()` once the Command is complete.
 * 
 * Calling `Fail()` will terminate any sequence in which the Command is operating, but
 * has no effect on Commands operating in parallel.
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

		/// Inidcates that the Command failed
		/// Used in sequential command groups to terminate the sequence
		void Fail();

		/// Inform the Command that further Execution has been terminated
		void Cancel ();

		/// The property set by `Retain` and `Release` to indicate whether the Command should be cleaned up on completion of the `Execute()` method. 
		bool retain{ get; }

		/// A payload injected into the Command. Most commonly, this an IEvent.
		object data{ get; set;}

		/// The property set to true by a Cancel() call.
		/// Use cancelled internally to determine if further execution is warranted, especially in
		/// asynchronous calls.
		bool cancelled{ get; set;}

		//The ordered id of this Command, used in sequencing to find the next Command.
		int sequenceId{ get; set; }
	}
}

