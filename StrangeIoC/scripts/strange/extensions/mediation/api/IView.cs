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
 * @class strange.extensions.mediation.api.IView
 * 
 * Monobehaviours must implement this interface in order to be injectable.
 * 
 * To contact the Context, the View must be able to find it. View handles this
 * with bubbling.
 */

using System;

namespace strange.extensions.mediation.api
{
	public interface IView
	{
		/// Indicates whether the View can work absent a context
		/// 
		/// Leave this value true most of the time. If for some reason you want
		/// a view to exist outside a context you can set it to false. The only
		/// difference is whether an error gets generated.
		bool requiresContext{ get; set;}
		
		/// Indicates whether this View  has been registered with a Context
		bool registeredWithContext{get; set;}

		/// Exposure to code of the registerWithContext (Inspector) boolean. If false, the View won't try to register.
		bool autoRegisterWithContext{ get; }

		bool enabled { get; }

		bool shouldRegister { get; }
	}
}

