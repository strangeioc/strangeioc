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
 * @interface strange.extensions.sequencer.api.ISequenceBinding
 * 
 * @deprecated
 */

using System;
using strange.extensions.command.api;
using strange.extensions.sequencer.api;
using strange.framework.api;

namespace strange.extensions.sequencer.api
{
	public interface ISequenceBinding : ICommandBinding
	{
		/// Declares that the Binding is a one-off. As soon as it's satisfied, it will be unmapped.
		new ISequenceBinding Once();

		/// Get/set the property set to `true` by `Once()`
		new bool isOneOff{ get; set;}

		new ISequenceBinding Bind<T>();
		new ISequenceBinding Bind(object key);
		new ISequenceBinding To<T>();
		new ISequenceBinding To(object o);
		new ISequenceBinding ToName<T> ();
		new ISequenceBinding ToName (object o);
		new ISequenceBinding Named<T>();
		new ISequenceBinding Named(object o);
	}
}

