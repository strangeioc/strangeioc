/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @interface strange.extensions.mediation.api.IMediationBinder
 * 
 * Interface for the Binder which maps Views to Mediators.
 * 
 * In a general sense, mediation is the act of using one thing
 * to separate and communicate between two others. mediation
 * in Strange refers to separating Views from the rest of the
 * application. The reason for doing this is not at all theoretical,
 * but highly practical. Views ypically come in two flavors:
 * the "component", which tends to be stable and dropped into
 * multiple, highly varied situations, and the highly volatile
 * "working" View, which changes rapidly according to last-minute
 * design considerations. In both cases, Mediators help you to insulate
 * the View from the app around it, and vice-versa. This yields
 * components that are easier to re-use, and working Views whose
 * chaos is structurally contained.
 * 
 * The MediationBinder quite simply Binds two (or more) classes. The Key
 * is the View class, the Value is one or more Mediators. Whenever the
 * View shows up, a corresponding Mediator joins it to buffer
 * View from app. The View doesn't know about the Mediator, nor
 * about the app beyond it. It simply establishes its API and waits
 * tp be told what to do. The Mediator, on the other hand, is allowed
 * to know quite a lot about the View and the app. It can be injected,
 * it has access to the common event bus, and can listen for and Dispatch
 * events. It is intended to be 'thin', that is, it should know just
 * enough to provide mediation. Leave logic, data storage, and View
 * behaviour to other classes.
 * 
 * In the context of Unity3D, the View is a MonoBehaviour attached
 * to a GameObject. The mediator is also a MonoBehaviour, so it has access
 * to all the usual things a MomoBehaviour would. Consider it a best
 * practice to attach only the View to your GameObject in Unity3D.
 * 
 * Finally, two words of warning.
 * First: Views may be injected. This allows you to provide a local Dispatcher,
 * a Configuration file, or other items that might come in handy. I
 * recommend very strongly that you do not inject the context-wide Dispatcher,
 * nor any other class involving the world beyond the View's immediate
 * area of interest. To do so risks negating the whole point of mediation.
 * 
 * Second: experience tells me that understanding what belongs in Mediator and
 * what in View is the trickiest part of this system. I recommend the
 * 'thin' class approach, i.e., nothing goes in the Mediator but what it needs
 * to mediate.
 */

using System;
using strange.framework.api;
using UnityEngine;

namespace strange.extensions.mediation.api
{
	public interface IMediationBinder : IBinder
	{
		/// An event that just happened, and the View it happened to.
		/// If the event was Awake, it will trigger creation of a mapped Mediator.
		void Trigger (MediationEvent evt, IView view);

		/// Recast binding as IMediationBinding.
		new IMediationBinding Bind<T> ();

		/// Porcelain for Bind<T> providing a little extra clarity and security.
		IMediationBinding BindView<T> () where T : MonoBehaviour;
	}
}

