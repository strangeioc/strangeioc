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
 * @class strange.extensions.mediation.impl.EventView
 * 
 * Injects a local event bus into this View. Intended
 * for local communication between the View and its
 * Mediator.
 * 
 * Caution: we recommend against injecting the context-wide
 * dispatcher into a View.
 */

using System;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.mediation.impl
{
	public class EventView : View
	{
		[Inject]
		public IEventDispatcher dispatcher{ get; set;}

	}
}

