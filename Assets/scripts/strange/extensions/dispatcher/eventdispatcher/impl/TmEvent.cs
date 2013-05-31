using strange.extensions.dispatcher.eventdispatcher.api;

namespace strange.extensions.dispatcher.eventdispatcher.impl
{
	public class TmEvent
	{
		public string type;
		public IEventDispatcher target;
		public object data;

		public TmEvent(string type, IEventDispatcher target, object data)
		{
			this.type = type;
			this.target = target;
			this.data = data;
		}
	}
}
