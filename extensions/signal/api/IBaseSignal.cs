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
 * @class strange.extensions.signal.api.IBaseSignal
 * 
 * The API that defines the use of a Signal.
 * 
 * Signals are a type-safe approach to communication that essentially replace the
 * standard EventDispatcher model. Signals can be injected/mapped just like any other
 * object -- as Singletons, as instances, or as values. Signals can even be mapped
 * across Contexts to provide an effective and type-safe way of communicating
 * between the parts of your application.
 * 
 * Additionally, the SignalCommandMap allows you to map Signals to Commands,
 * in just the same way as you would map Events to Commands.
 * Note that Signals bind their parameters to Command injections by comparing Types
 * and do not understand named injections. Therefore, in order to Bind a Command's injections to a Signal,
 * PARAMETERS/INJECTION PAIRS MUST BE OF UNIQUE TYPES. So while Signals themselves are
 * allowed to have two parameters of the same Type, Signals mapped to Commands must never do
 * this.
 * 
 * Signals in Strange use the Action Class as the underlying mechanism for type safety.
 * Unity's C# implementation currently allows up to FOUR parameters in an Action, therefore
 * SIGNALS ARE LIMITED TO FOUR PARAMETERS. If you require more than four, consider
 * creating a value object to hold additional values.
 * 
 * Example uses in strange.extensions.signal.impl.Signal 
 * 
 * @see strange.extensions.signal.impl.BaseSignal
 * @see strange.extensions.signal.impl.Signal
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.signal.api
{
	public interface IBaseSignal
	{
		/// Instruct a Signal to call on all its registered listeners
		void Dispatch(object[] args);

		/// Attach a callback to this Signal
		/// The callback parameters must match the Types and order which were
		/// originally assigned to the Signal on its creation
		void AddListener(Action<IBaseSignal, object[]> callback);

		/// Attach a callback to this Signal for the duration of exactly one Dispatch
		/// The callback parameters must match the Types and order which were
		/// originally assigned to the Signal on its creation, and the callback
		/// will be removed immediately after the Signal dispatches
		void AddOnce (Action<IBaseSignal, object[]> callback);

		/// Remove a callback from this Signal
		void RemoveListener(Action<IBaseSignal, object[]> callback);

		/// Returns a List<System.Type> representing the Types bindable to this Signal
		List<Type> GetTypes();
	}
}
