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

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class TmEvent
	{
		public string type;
		public IEventDispatcher target;
		public object data;

		/// Construct a TmEvent
		public TmEvent(string type, IEventDispatcher target, object data)
		{
			this.type = type;
			this.target = target;
			this.data = data;
		}
	}
}
