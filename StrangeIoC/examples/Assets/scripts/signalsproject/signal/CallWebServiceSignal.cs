/// A Signal for triggering a CallWebServiceCommand
/// 
/// string Just some random text to demonstrate a value being Injected into a Command

using System;
using strange.extensions.signal.impl;

namespace strange.examples.signals
{
	public class CallWebServiceSignal : Signal<string>
	{
	}
}

