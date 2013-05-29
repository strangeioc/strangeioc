using System;
using UnityEngine;
using NUnit.Framework;
using babel.extensions.injector.api;
using babel.extensions.injector.impl;
using babel.extensions.mediation.api;
using babel.extensions.mediation.impl;

namespace babel.unittests
{


	[TestFixture()]
	public class TestView
	{

		IInjectionBinder injectionBinder;

		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder();
			injectionBinder.Bind<IMediationBeacon> ().To<IMediationBeacon>().AsSingleton();
		}

		[Test]
		public void TestAwake ()
		{
			//GameObject go = new GameObject ("go");
			//go.AddComponent<View> ();
			//go.SetActive (true);

		}

		//[Test]
		//public void Tes
		//IMediationBeacon beacon = injectionBinder.GetInstance<IMediationBeacon> () as IMediationBeacon;
	}
}

