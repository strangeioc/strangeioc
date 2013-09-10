using System;
using NUnit.Framework;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.unittests
{
	[TestFixture()]
	public class TestEventBinding
	{
		private const int INIT_VALUE = 42;
		private int confirmationValue = 42;


		[Test]
		public void TestMapNoArgumentCallback ()
		{
			confirmationValue = INIT_VALUE;
			IEventBinding binding = new EventBinding ();
			binding.Key (SomeEnum.ONE).To (noArgumentCallback);
			EventCallbackType type = binding.typeForCallback (noArgumentCallback);
			object[] value = binding.value as object[];
			Delegate extracted =  value[0] as Delegate;

			Assert.AreEqual (EventCallbackType.NO_ARGUMENTS, type);

			extracted.DynamicInvoke(new object[0]);
			//Calling the method should change the confirmationValue
			Assert.AreNotEqual (confirmationValue, INIT_VALUE);
		}

		private void noArgumentCallback()
		{
			confirmationValue *= 2;
		}

		[Test]
		public void TestMapOneArgumentCallback ()
		{
			confirmationValue = INIT_VALUE;
			IEventBinding binding = new EventBinding ();
			binding.Key (SomeEnum.ONE).To (oneArgumentCallback);
			EventCallbackType type = binding.typeForCallback (oneArgumentCallback);
			object[] value = binding.value as object[];
			Delegate extracted =  value[0] as Delegate;

			Assert.AreEqual (EventCallbackType.ONE_ARGUMENT, type);

			object[] parameters = new object[1];
			parameters [0] = new TestEvent("TEST", null, INIT_VALUE);
			extracted.DynamicInvoke(parameters);
			//Calling the method should change the confirmationValue
			Assert.AreEqual (confirmationValue, INIT_VALUE * INIT_VALUE);
		}

		private void oneArgumentCallback(IEvent o)
		{
			confirmationValue *= (int)o.data;
		}


        class TestEvent : IEvent
        {
            public object type{ get; set;}
		    public IEventDispatcher target{ get; set;}
		    public object data{ get; set;}

            
            public TestEvent(object type, IEventDispatcher target, object data)
		    {
			    this.type = type;
			    this.target = target;
			    this.data = data;
		    }
        }
	}
}

