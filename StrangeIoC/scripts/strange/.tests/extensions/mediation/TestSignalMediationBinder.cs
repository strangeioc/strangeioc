using System;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.mediation;
using strange.framework.api;

namespace strange.unittests
{
	[TestFixture]
	public class TestSignalMediationBinder
	{
		private OverriddenSignalMediationBinder binder;
		private InjectionBinder injectionBinder;
		

		[SetUp]
		public void Setup()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IInstanceProvider>().Bind<IInjectionBinder>().ToValue(injectionBinder);
			injectionBinder.Bind<OverriddenSignalMediationBinder>().ToSingleton();
			injectionBinder.Bind<OneArgSignal>().ToSingleton();

			binder = injectionBinder.GetInstance<OverriddenSignalMediationBinder>(); //get an injected version to make our life easier

			injectionBinder.Bind<SignalMediator>().ToSingleton();
		}

		[TearDown]
		public void Teardown()
		{
			SignalMediator.Value = 0;
			SignalMediatorTwo.Value = 0;
		}

		[Test]
		public void HandleDelegate_Add_AddsOneListener()
		{
			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();
			
			Assert.AreEqual(1, oneArgSignal.listener.GetInvocationList().Length); 
			binder.TestHandleDelegate(mediator, mediator.GetType(), true);
			Assert.AreEqual(2, oneArgSignal.listener.GetInvocationList().Length);
		}

		[Test]
		public void HandleDelegate_Add_PrivateMethod()
		{
			injectionBinder.Bind<SignalMediatorPrivate>().ToSingleton();
			SignalMediatorPrivate mediator = injectionBinder.GetInstance<SignalMediatorPrivate>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			Assert.AreEqual(1, oneArgSignal.listener.GetInvocationList().Length);
			binder.TestHandleDelegate(mediator, mediator.GetType(), true);
			Assert.AreEqual(2, oneArgSignal.listener.GetInvocationList().Length);
		}

		[Test]
		public void HandleDelegate_Add_ListenerCalledOnDispatch()
		{
			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			binder.TestHandleDelegate(mediator, mediator.GetType(), true);

			Assert.AreEqual(0, SignalMediator.Value);
			oneArgSignal.Dispatch(2);
			Assert.AreEqual(2, SignalMediator.Value);
		}

		[Test]
		public void HandleDelegate_AddTwo_BothListenersAreAdded()
		{
			injectionBinder.Bind<SignalMediatorTwo>().ToSingleton();

			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			SignalMediatorTwo mediatorTwo = injectionBinder.GetInstance<SignalMediatorTwo>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			Assert.AreEqual(1, oneArgSignal.listener.GetInvocationList().Length);
			
			binder.TestHandleDelegate(mediator, mediator.GetType(), true);
			binder.TestHandleDelegate(mediatorTwo, mediatorTwo.GetType(), true);
			Assert.AreEqual(3, oneArgSignal.listener.GetInvocationList().Length);
		}

		[Test]
		public void HandleDelegate_AddTwo_BothListenersCalledOnDispatch()
		{
			injectionBinder.Bind<SignalMediatorTwo>().ToSingleton();

			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			SignalMediatorTwo mediatorTwo = injectionBinder.GetInstance<SignalMediatorTwo>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			binder.TestHandleDelegate(mediator, mediator.GetType(), true);
			binder.TestHandleDelegate(mediatorTwo, mediatorTwo.GetType(), true);

			Assert.AreEqual(0, SignalMediator.Value);
			Assert.AreEqual(0, SignalMediatorTwo.Value);
			oneArgSignal.Dispatch(2);
			Assert.AreEqual(2, SignalMediator.Value);
			Assert.AreEqual(2, SignalMediatorTwo.Value);
		}


		[Test]
		public void HandleDelegate_Remove_RemovesListener()
		{
			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			binder.TestHandleDelegate(mediator, mediator.GetType(), true); //add
			binder.TestHandleDelegate(mediator, mediator.GetType(), false); //remove
			Assert.AreEqual(1, oneArgSignal.listener.GetInvocationList().Length);
		}

		[Test]
		public void HandleDelegate_Remove_ListenerNotCalledOnDispatch()
		{
			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			binder.TestHandleDelegate(mediator, mediator.GetType(), true); //add
			binder.TestHandleDelegate(mediator, mediator.GetType(), false); //remove

			oneArgSignal.Dispatch(2);
			Assert.AreEqual(0, SignalMediator.Value);
		}


		[Test]
		public void HandleDelegate_Remove_RemovesCorrectListener()
		{
			injectionBinder.Bind<SignalMediatorTwo>().ToSingleton();

			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			SignalMediatorTwo mediatorTwo = injectionBinder.GetInstance<SignalMediatorTwo>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			binder.TestHandleDelegate(mediator, mediator.GetType(), true);
			binder.TestHandleDelegate(mediatorTwo, mediatorTwo.GetType(), true);


			binder.TestHandleDelegate(mediator, mediator.GetType(), false);

			oneArgSignal.Dispatch(2);

			
			//remove
			Assert.AreEqual(0, SignalMediator.Value);
			Assert.AreEqual(2, SignalMediatorTwo.Value);
		}

		[Test]
		public void HandleDelegate_Remove_AddOrderDoesntMatter()
		{
			injectionBinder.Bind<SignalMediatorTwo>().ToSingleton();

			SignalMediator mediator = injectionBinder.GetInstance<SignalMediator>();
			SignalMediatorTwo mediatorTwo = injectionBinder.GetInstance<SignalMediatorTwo>();
			var oneArgSignal = injectionBinder.GetInstance<OneArgSignal>();

			binder.TestHandleDelegate(mediatorTwo, mediatorTwo.GetType(), true);
			binder.TestHandleDelegate(mediator, mediator.GetType(), true);


			binder.TestHandleDelegate(mediator, mediator.GetType(), false);

			oneArgSignal.Dispatch(2);


			//remove
			Assert.AreEqual(0, SignalMediator.Value);
			Assert.AreEqual(2, SignalMediatorTwo.Value);
		}


		//Hacky, but due to our restrictions dealing with UnityEngine and icall, we need to expose these methods to test them properly
		//Simply put, we cannot call the usual public entry points (Trigger), so we have to test protected methods.
		class OverriddenSignalMediationBinder : SignalMediationBinder
		{
			public void TestHandleDelegate(object mono, Type mediatorType, bool toAdd)
			{
				HandleDelegates(mono, mediatorType, toAdd);
			}
		}
	}

	
}
