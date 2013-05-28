using System;
using babel.extensions.dispatcher.eventdispatcher.api;

namespace babel.examples.myfirstproject
{
	public interface IExampleService
	{
		void Request(string url);
		IEventDispatcher dispatcher{get;set;}
	}
}

