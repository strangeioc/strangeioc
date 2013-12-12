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
 * @interface strange.extensions.context.api.IContextView
 * 
 * The ContextView is the entry point to the application.
 * 
 * In a standard MVCSContext setup for Unity3D, it is a MonoBehaviour
 * attached to a GameObject at the very top of of your application.
 * It's most important role is to instantiate and call `Start()` on the Context.
 */

using System;
using strange.extensions.mediation.api;

namespace strange.extensions.context.api
{
	public interface IContextView : IView
	{
		/// Get and set the Context
		IContext context{get;set;}
	}
}

