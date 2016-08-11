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
 * @class BindAs
 * 
 * The `[BindAs]` attribute allows a class to be recognized as one of it's implementing or base types.
 * 
 * Example:

		[BindAs(typeof(IMainView))]
		public class MainView : View, IMainView {
			...
		}
*
*  This is useful in using the mediationBinder in a more generic fashion to allow for better abstraction
*  at runtime.
*  
*  Example:

		mediationBinder.Bind<MainView>().To<MainViewMediator>();

		// now bceomes
		mediationBinder.Bind<IMainView>().To<MainViewMediator>();
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.mediation.api
{
	[AttributeUsage(AttributeTargets.Property,
		AllowMultiple = false,
		Inherited = true)]
	public class MediateAs : Attribute
	{
		public MediateAs(Type t) {
			MediateAsType = t;
		}

		public Type MediateAsType { get; set; }
	}
}