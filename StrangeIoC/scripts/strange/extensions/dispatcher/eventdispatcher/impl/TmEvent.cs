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
 * @class strange.extensions.dispatcher.eventdispatcher.impl.TmEvent
 * 
 * The standard Event object for IEventDispatcher.
 * 
 * The TmEvent has three proeprties:
 * <ul>
 *  <li>type - The key for the event trigger</li>
 *  <li>target - The Dispatcher that fired the event</li>
 *  <li>data - An arbitrary payload</li>
 * </ul>
 */

using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.pool.api;

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class TmEvent : IEvent, IPoolable
	{
		public object type{ get; set; }
		public IEventDispatcher target{ get; set; }
		public object data{ get; set; }

		protected int retainCount;

		public TmEvent()
		{
		}

		/// Construct a TmEvent
		public TmEvent(object type, IEventDispatcher target, object data)
		{
			this.type = type;
			this.target = target;
			this.data = data;
		}

		#region IPoolable implementation

		public void Restore ()
		{
			this.type = null;
			this.target = null;
			this.data = null;
		}

		public void Retain()
		{
			retainCount++;
			System.Console.WriteLine ("Retain: " + retainCount);
		}

		public void Release()
		{
			retainCount--;
			System.Console.WriteLine ("Release: " + retainCount);
			if (retainCount == 0)
			{
				target.ReleaseEvent (this);
			}
		}

		public bool retain{ 
			get
			{
				return retainCount > 0;
			}
		}

		#endregion
	}
}
