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
			binding.Bind (SomeEnum.ONE).To (noArgumentCallback);
			EventCallbackType type = binding.TypeForCallback (noArgumentCallback);
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
			binding.Bind (SomeEnum.ONE).To (oneArgumentCallback);
			EventCallbackType type = binding.TypeForCallback (oneArgumentCallback);
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
            public object Type{ get; set;}
		    public IEventDispatcher Target{ get; set;}
		    public object Data{ get; set;}

            
            public TestEvent(object type, IEventDispatcher target, object data)
		    {
			    this.Type = type;
			    this.Target = target;
			    this.Data = data;
		    }

			public object type{ get{ return Type;} set{ Type = value; }}
			public IEventDispatcher target{ get { return Target;} set{ Target = value; }}
			public object data{ get { return Data;} set{ Data = value; }}
        }
	}
}

