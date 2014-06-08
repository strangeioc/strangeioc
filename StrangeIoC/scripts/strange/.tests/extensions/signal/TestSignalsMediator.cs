using NUnit.Framework;
using strange.extensions.context.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace strange.unittests
{
    [TestFixture()]
	public class TestSignalsMediator : BaseUnitTest
    {
        class MockView : View
        {
            public Signal noParmSignal = new Signal();
            public Signal<int> oneParamSignal = new Signal<int>();
            public Signal<int, string> twoParamSignal = new Signal<int, string>();
            public Signal<int, string, bool> threeParamSignal = new Signal<int, string, bool>();
            public Signal<int, string, bool, float> fourParamSignal = new Signal<int, string, bool, float>();
        }

        class MockMediator : SignalsMediator
        {
            [Inject]
            public MockView View { get; set; }

            public bool noParamCalled = false;

            public bool oneParamCalled = false;
            public int oneParamValue = -1;

            public bool twoParamCalled = false;
            public int twoParamValueA = -1;
            public string twoParamValueB = null;

            public bool threeParamCalled = false;
            public int threeParamValueA = -1;
            public string threeParamValueB = null;
            public bool threeParamValueC = false;

            public bool fourParamCalled = false;
            public int fourParamValueA = -1;
            public string fourParamValueB = null;
            public bool fourParamValueC = false;
            public float fourParamValueD = -1.1f;

            public override void OnRegister()
            {
                RegisterListener(View.noParmSignal, () => noParamCalled = true);

                RegisterListener(View.oneParamSignal, a =>
                {
                    oneParamCalled = true;
                    oneParamValue = a;
                });

                RegisterListener(View.twoParamSignal, (a, b) =>
                {
                    twoParamCalled = true;
                    twoParamValueA = a;
                    twoParamValueB = b;
                });

                RegisterListener(View.threeParamSignal, (a, b, c) =>
                {
                    threeParamCalled = true;
                    threeParamValueA = a;
                    threeParamValueB = b;
                    threeParamValueC = c;
                });

                RegisterListener(View.fourParamSignal, (a, b, c, d) =>
                {
                    fourParamCalled = true;
                    fourParamValueA = a;
                    fourParamValueB = b;
                    fourParamValueC = c;
                    fourParamValueD = d;
                });
            }
        }

        private InjectionBinder injector;
        private MockMediator mediator;
        private MockView view;

        [SetUp]
        public void Init()
        {
            view = new MockView();
            injector = new InjectionBinder();
            injector.Bind<GameObject>().ToName(ContextKeys.CONTEXT_VIEW).ToValue(new GameObject());
            injector.Bind<MockView>().ToValue(view);
            injector.Bind<MockMediator>().ToSingleton();
            injector.Bind<IInjectionBinder>().ToValue(injector);
            mediator = injector.GetInstance<MockMediator>();
            mediator.OnRegister();
        }

        [Test]
        public void NothingCalled()
        {
            Assert.AreEqual(false, mediator.noParamCalled);
            Assert.AreEqual(false, mediator.oneParamCalled);
            Assert.AreEqual(false, mediator.twoParamCalled);
            Assert.AreEqual(false, mediator.threeParamCalled);
            Assert.AreEqual(false, mediator.fourParamCalled);
        }

        [Test]
        public void CallbacksGetCalled()
        {
            view.noParmSignal.Dispatch();
            Assert.AreEqual(true, mediator.noParamCalled);
            view.oneParamSignal.Dispatch(12345);
            Assert.AreEqual(true, mediator.oneParamCalled);
            Assert.AreEqual(12345, mediator.oneParamValue);
            view.twoParamSignal.Dispatch(12345,"hello");
            Assert.AreEqual(true, mediator.twoParamCalled);
            Assert.AreEqual(12345, mediator.twoParamValueA);
            Assert.AreEqual("hello", mediator.twoParamValueB);
            view.threeParamSignal.Dispatch(12345, "hello", true);
            Assert.AreEqual(true, mediator.threeParamCalled);
            Assert.AreEqual(12345, mediator.threeParamValueA);
            Assert.AreEqual("hello", mediator.threeParamValueB);
            Assert.AreEqual(true, mediator.threeParamValueC);
            view.fourParamSignal.Dispatch(12345, "hello", true, 12.345f);
            Assert.AreEqual(true, mediator.fourParamCalled);
            Assert.AreEqual(12345, mediator.fourParamValueA);
            Assert.AreEqual("hello", mediator.fourParamValueB);
            Assert.AreEqual(true, mediator.fourParamValueC);
            Assert.AreEqual(12.345f, mediator.fourParamValueD);
        }

        [Test]
        public void CallbacksDoNotGetCalledAfterRemoval()
        {
            mediator.OnPreRemove();
            view.noParmSignal.Dispatch();
            Assert.AreEqual(false, mediator.noParamCalled);
            view.oneParamSignal.Dispatch(12345);
            Assert.AreEqual(false, mediator.oneParamCalled);
            view.twoParamSignal.Dispatch(12345, "hello");
            Assert.AreEqual(false, mediator.twoParamCalled);
            view.threeParamSignal.Dispatch(12345, "hello", true);
            Assert.AreEqual(false, mediator.threeParamCalled);
            view.fourParamSignal.Dispatch(12345, "hello", true, 12.345f);
            Assert.AreEqual(false, mediator.fourParamCalled);
        }
    }
    }
}
