using System;
using NUnit.Framework;
using strange.framework.api;
using strange.extensions.injector.impl;
using strange.extensions.mediation.impl;
using strange.extensions.mediation.api;
using System.Collections.Generic;
using strange.framework.impl;


namespace strange.unittests
{
	[TestFixture()]
	public class TestAbstractMediationBinder
	{
		private TestMediationBinder mediationBinder;
		private InjectionBinder injectionBinder;


		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder ();
			mediationBinder = new TestMediationBinder ();
			mediationBinder.injectionBinder = injectionBinder;
		}

		[Test]
		public void TestRawBindingIsMediationBinding()
		{
			IBinding binding = mediationBinder.GetRawBinding ();
			Assert.IsInstanceOf<IMediationBinding> (binding);
		}

		[Test]
		public void TestAwakeTriggersMappingAndInjection()
		{
			mediationBinder.Bind<TestView> ().To<TestMediator> ();
			injectionBinder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();

			TestView view = new TestView ();
			mediationBinder.Trigger (MediationEvent.AWAKE, view);

			Assert.IsTrue (view.registeredWithContext);
			Assert.IsNotNull (view.testInjection);
			TestMediator mediator = mediationBinder.mediators [view] as TestMediator;
			Assert.AreEqual (1, mediationBinder.mediators.Count);
			Assert.IsNotNull (mediator);
			Assert.IsInstanceOf<TestMediator> (mediator);

			Assert.IsTrue (mediator.preregistered);
			Assert.IsTrue (mediator.registered);
			Assert.IsFalse (mediator.removed);
		}

		[Test]
		public void TestAwakeTriggersInjectionForUnmappedView()
		{
			injectionBinder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();

			TestView view = new TestView ();
			mediationBinder.Trigger (MediationEvent.AWAKE, view);

			Assert.IsNotNull (view.testInjection);
			TestMediator mediator = null;
			if (mediationBinder.mediators.ContainsKey(view))
			{
				mediator = mediationBinder.mediators [view] as TestMediator;
			}
			Assert.AreEqual (0, mediationBinder.mediators.Count);
			Assert.IsNull (mediator);
		}

		[Test]
		public void TestEnableTriggersMediatorEnabled()
		{
			mediationBinder.Bind<TestView> ().To<TestMediator> ();
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

			TestView view = new TestView ();
			mediationBinder.Trigger(MediationEvent.AWAKE, view);

			TestMediator mediator = mediationBinder.mediators[view] as TestMediator;

			mediationBinder.Trigger(MediationEvent.ENABLED, view);
			Assert.IsTrue(mediator.enabled);
		}

		[Test]
		public void TestDisableTriggersMediatorDisabled()
		{
			mediationBinder.Bind<TestView> ().To<TestMediator> ();
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

			TestView view = new TestView ();
			mediationBinder.Trigger(MediationEvent.AWAKE, view);

			TestMediator mediator = mediationBinder.mediators[view] as TestMediator;

			mediationBinder.Trigger(MediationEvent.DISABLED, view);
			Assert.IsTrue(mediator.disabled);
		}

		[Test]
		public void TestDestroyedTriggersUnmapping()
		{
			mediationBinder.Bind<TestView> ().To<TestMediator> ();
			injectionBinder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();

			TestView view = new TestView ();
			mediationBinder.Trigger (MediationEvent.AWAKE, view);

			TestMediator mediator = mediationBinder.mediators [view] as TestMediator;

			mediationBinder.Trigger (MediationEvent.DESTROYED, view);
			Assert.IsTrue (mediator.removed);
			Assert.AreEqual (0, mediationBinder.mediators.Count);
		}

		[Test]
		public void TestErrorIfClassMappedToItself()
		{
			mediationBinder.Bind<TestView> ().To<TestView> ();
			injectionBinder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();

			TestView view = new TestView ();

			TestDelegate testDelegate = delegate
			{
				mediationBinder.Trigger (MediationEvent.AWAKE, view);
			};
			MediationException ex = Assert.Throws<MediationException>(testDelegate); //Because we've mapped view to self
			Assert.AreEqual (MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW, ex.type);
		}

		[Test]
		public void TestInjectViews()
		{
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

			TestView view = new TestView();
			IView one = new TestView();
			IView two = new TestView();
			IView three = new TestView();

			IView[] views =
			{
				view,
				one,
				two,
				three
			};

			view.Views = views;

			mediationBinder.TestInjectViewAndChildren(view);

			Assert.AreEqual(true, one.registeredWithContext);
			Assert.AreEqual(true, two.registeredWithContext);
			Assert.AreEqual(true, three.registeredWithContext);

		}

		[Test]
		public void TestSimpleRuntimeBinding()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.TestView\",\"To\":\"strange.unittests.TestMediator\"}]";

			mediationBinder.ConsumeBindings (jsonString);

			IBinding binding = mediationBinder.GetBinding<TestView> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IMediationBinding).key, typeof(TestView));

			object[] value = (binding as IMediationBinding).value as object[];
			Assert.AreEqual (value[0], typeof(TestMediator));
		}

		[Test]
		public void TestMultipleRuntimeBindings()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.TestView\",\"To\":\"strange.unittests.TestMediator\"}, {\"Bind\":\"strange.unittests.TestView2\",\"To\":\"strange.unittests.TestMediator2\"}]";

			mediationBinder.ConsumeBindings (jsonString);

			IBinding binding = mediationBinder.GetBinding<TestView> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IMediationBinding).key, typeof(TestView));

			object[] value = (binding as IMediationBinding).value as object[];
			Assert.AreEqual (value[0], typeof(TestMediator));

			IBinding binding2 = mediationBinder.GetBinding<TestView2> ();
			Assert.NotNull (binding2);
			Assert.AreEqual ((binding2 as IMediationBinding).key, typeof(TestView2));

			object[] value2 = (binding2 as IMediationBinding).value as object[];
			Assert.AreEqual (value2[0], typeof(TestMediator2));
		}

		[Test]
		public void TestBindOneViewToManyMediators()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.TestView\",\"To\":[\"strange.unittests.TestMediator\",\"strange.unittests.TestMediator2\"]}]";

			mediationBinder.ConsumeBindings (jsonString);

			IBinding binding = mediationBinder.GetBinding<TestView> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IMediationBinding).key, typeof(TestView));

			object[] value = (binding as IMediationBinding).value as object[];
			Assert.Contains (typeof(TestMediator), value);
			Assert.Contains (typeof(TestMediator2), value);
		}

		[Test]
		public void TestBindViewToMediatorSyntax()
		{
			string jsonString = "[{\"BindView\":\"strange.unittests.TestView\",\"ToMediator\":[\"strange.unittests.TestMediator\",\"strange.unittests.TestMediator2\"]}]";

			mediationBinder.ConsumeBindings (jsonString);

			IBinding binding = mediationBinder.GetBinding<TestView> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IMediationBinding).key, typeof(TestView));

			object[] value = (binding as IMediationBinding).value as object[];
			Assert.Contains (typeof(TestMediator), value);
			Assert.Contains (typeof(TestMediator2), value);
		}

		[Test]
		public void TestBindToAbstraction()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.TestView2\",\"To\":\"strange.unittests.TestMediator\",\"ToAbstraction\":\"strange.unittests.TestView\"}]";

			mediationBinder.ConsumeBindings (jsonString);
			injectionBinder.Bind<ClassToBeInjected> ().To<ClassToBeInjected> ();


			IBinding binding = mediationBinder.GetBinding<TestView2> ();
			Assert.NotNull (binding);
			Assert.AreEqual ((binding as IMediationBinding).key, typeof(TestView2));

			object[] value = (binding as IMediationBinding).value as object[];
			Assert.Contains (typeof(TestMediator), value);

			TestView2 view = new TestView2 ();
			mediationBinder.Trigger (MediationEvent.AWAKE, view);


			Assert.IsTrue (view.registeredWithContext);
			Assert.IsNotNull (view.testInjection);
			TestMediator mediator = mediationBinder.mediators [view] as TestMediator;
			Assert.AreEqual (1, mediationBinder.mediators.Count);
			Assert.IsNotNull (mediator);
			Assert.IsInstanceOf<TestMediator> (mediator);
			Assert.IsInstanceOf<TestView2> (mediator.view);
		}

		[Test]
		public void TestThrowsErrorOnUnresolvedView()
		{
			string jsonString = "[{\"Bind\":\"TestView\",\"To\":\"strange.unittests.TestMediator\"}]";

			TestDelegate testDelegate = delegate
			{
				mediationBinder.ConsumeBindings (jsonString);
			};

			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because the Bind value isn't fully qualified
			Assert.AreEqual (BinderExceptionType.RUNTIME_NULL_VALUE, ex.type);
		}

		[Test]
		public void TestThrowsErrorOnUnresolvedMediator()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.TestView\",\"To\":\"TestMediator\"}]";

			TestDelegate testDelegate = delegate
			{
				mediationBinder.ConsumeBindings (jsonString);
			};

			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because the To value isn't fully qualified
			Assert.AreEqual (BinderExceptionType.RUNTIME_NULL_VALUE, ex.type);
		}

		[Test]
		public void TestThrowsErrorOnUnresolvedAbstraction()
		{
			string jsonString = "[{\"Bind\":\"strange.unittests.TestView2\",\"To\":\"strange.unittests.TestMediator\",\"ToAbstraction\":\"TestView\"}]";

			TestDelegate testDelegate = delegate
			{
				mediationBinder.ConsumeBindings (jsonString);
			};

			BinderException ex = Assert.Throws<BinderException>(testDelegate); //Because the Abstraction value isn't fully qualified
			Assert.AreEqual (BinderExceptionType.RUNTIME_NULL_VALUE, ex.type);
		}

		[Test]
		public void TestMediatorEnabledAtCreationIfEnabled()
		{
			mediationBinder.Bind<TestView>().To<TestMediator>();
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

			TestView view = new TestView();
			mediationBinder.Trigger(MediationEvent.AWAKE, view);

			TestMediator mediator = mediationBinder.mediators[view] as TestMediator;
			Assert.True(mediator.enabled);
		}

		[Test]
		public void TestMediatorNotEnabledAtCreatioIfDisabledn()
		{
			mediationBinder.Bind<TestView>().To<TestMediator>();
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

			TestView view = new TestView();
			view.enabled = false;
			mediationBinder.Trigger(MediationEvent.AWAKE, view);

			TestMediator mediator = mediationBinder.mediators[view] as TestMediator;
			Assert.False(mediator.enabled);
		}

		[Test]
		public void TestInjectViewAndChildrenInjectsChildrenBeforeParentWhenAllBelongToSameGameObject()
		{
			injectionBinder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

			List<TestView> order = new List<TestView>();
			Action<TestView> postInjectAction = (TestView v) =>
			{
				order.Add(v);
			};

			TestView parent = new TestView();
			parent.PostInjectionAction = postInjectAction;

			TestView one = new TestView();
			one.PostInjectionAction = postInjectAction;

			TestView two = new TestView();
			two.PostInjectionAction = postInjectAction;

			IView[] views =
			{
				parent,
				one,
				two
			};


			/*
			 * In an scenario in which:
			 * A game object contains a ParentView (parent) and a ChildView (child) scripts, and 
			 * parent is connected to child (ivar connected from the unity editor)
			 * 
			 * THEN:
			 * Both parent and child will return the same array of IView when MediationBinder.GetViews() is called
			*/

			parent.Views = views;
			one.Views = views;
			two.Views = views;

			mediationBinder.TestInjectViewAndChildren(parent);

			Assert.AreEqual(true, one.registeredWithContext);
			Assert.AreEqual(true, two.registeredWithContext);

			int parentIndex = order.IndexOf(parent);
			int firstChildIndex = order.IndexOf(one);
			int secondChildIndex = order.IndexOf(two);

			//One and Two must be injected before parent, thus, their index in the order list should be lower
			Assert.That(parentIndex > firstChildIndex);
			Assert.That(parentIndex > secondChildIndex);
		}
	}


	class TestMediationBinder : AbstractMediationBinder
	{

		public Dictionary<IView, IMediator> mediators = new Dictionary<IView, IMediator>();

		protected override IView[] GetViews(IView view)
		{
			TestView testView = view as TestView;
			return testView.Views;
		}

		protected override bool HasMediator(IView view, Type mediatorType)
		{
			TestView testView = view as TestView;
			return testView.HasMediator;
		}

		override protected object CreateMediator(IView view, Type mediatorType)
		{
			IMediator mediator = new TestMediator ();
			mediators.Add (view, mediator);
			return mediator;
		}

		override protected object DestroyMediator(IView view, Type mediatorType)
		{
			IMediator mediator = null;
			if (mediators.ContainsKey(view))
			{
				mediator = mediators[view];
				mediators.Remove(view);
				mediator.OnRemove ();
			}
			return mediator;
		}

		protected override object EnableMediator(IView view, Type mediatorType)
		{
			IMediator mediator;
			if (mediators.TryGetValue(view, out mediator))
			{
				mediator.OnEnabled();
			}
			return mediator;

		}

		protected override object DisableMediator(IView view, Type mediatorType)
		{
			IMediator mediator;
			if (mediators.TryGetValue(view, out mediator))
			{
				mediator.OnDisabled();
			}
			return mediator;
		}

		protected override void ThrowNullMediatorError (Type viewType, Type mediatorType)
		{
			throw new MediationException("The view: " + viewType.ToString() + " is mapped to mediator: " + mediatorType.ToString() + ". AddComponent resulted in null, which probably means " + mediatorType.ToString().Substring(mediatorType.ToString().LastIndexOf(".") + 1) + " is not a MonoBehaviour.", MediationExceptionType.NULL_MEDIATOR);
		}

		public void TestInjectViewAndChildren(IView view)
		{
			InjectViewAndChildren(view);
		}

		public void TestPerformKeyValueBindings(List<object> keyList, List<object> valueList)
		{
			performKeyValueBindings(keyList, valueList);
		}
	}

	class TestView : IView
	{
		[Inject]
		public ClassToBeInjected testInjection { get; set; }

		public TestView()
		{
			enabled = true;
			Views = new IView[]
			{
				this
			};
		}

		public string name { get; set; }
		public Action<TestView> PostInjectionAction { get; set; }

		[PostConstruct]
		public void TestPostInjection()
		{
			if (this.PostInjectionAction != null)
			{
				Console.WriteLine("POST INJECT: " + name);
				this.PostInjectionAction(this);
			}
		}


		#region IView implementation

		private bool _requiresContext;
		private bool _registeredWithContext;

		public IView[] Views = {};
		public bool HasMediator = false;

		public bool requiresContext
		{
			get
			{
				return _requiresContext;
			}
			set
			{
				_requiresContext = value;
			}
		}

		public bool registeredWithContext
		{
			get
			{
				return _registeredWithContext;
			}
			set
			{
				_registeredWithContext = value;
			}
		}

		public bool autoRegisterWithContext
		{
			get
			{
				return true;
			}
		}

		public bool enabled { get; set; }

		#endregion
	}
	class TestView2 : TestView{}

	class TestMediator : IMediator
	{

		[Inject]
		public TestView view{ get; set; }

		public bool preregistered = false;
		public bool registered = false;
		public bool removed = false;
		public bool enabled = false;
		public bool disabled = false;

		#region IMediator implementation

		public void PreRegister ()
		{
			preregistered = true;
		}

		public void OnRegister ()
		{
			registered = true;
		}

		public void OnRemove ()
		{
			removed = true;
		}

		public void OnEnabled ()
		{
			enabled = true;
		}

		public void OnDisabled ()
		{
			disabled = true;
		}

		public UnityEngine.GameObject contextView
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
	class TestMediator2 : TestMediator{}
}

