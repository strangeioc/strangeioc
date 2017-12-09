/*
 * Copyright 2015 StrangeIoC
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
 * @class ListensTo
 * 
 * The `[ListensTo]` attribute provides a shortcut for adding
 * Signal-to-Method binding with Mediators.
 * Example:

		[ListensTo(typeof(PlayerUpdateSignal))]
		public void onPlayerUpdate(int hitpoints)
		{
			//do some stuff
		}
 *
 * The above example performs PlayerUpdateSignal.AddListener(onPlayerUpdate)
 * within OnRegister, and PlayerUpdateSignal.RemoveListener(onPlayerUpdate)
 * within OnRemove.
 *
 * NOTE: THE LISTENING METHOD MUST BE MARKED PUBLIC. Private and protected
 * methods are not scanned and the ListensTo attribute will be silently
 * ignored.
 */


using System;

[AttributeUsage(AttributeTargets.Method,
		AllowMultiple = false,
		Inherited = true)]
public class ListensTo : Attribute
{
	public ListensTo(){}

	public ListensTo(Type t)
	{
		type = t;
	}

	public Type type {get; set;}
}
