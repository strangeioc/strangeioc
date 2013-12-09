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
 * @class strange.extensions.mediation.impl.View
 * 
 * Parent class for all your Views. Extends MonoBehaviour.
 * Bubbles its Awake, Start and OnDestroy events to the
 * ContextView, which allows the Context to know when these
 * critical moments occur in the View lifecycle.
 */

using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.mediation.api;

namespace strange.extensions.mediation.impl
{
	public class View : MonoBehaviour, IView
	{
		/// Leave this value true most of the time. If for some reason you want
		/// a view to exist outside a context you can set it to false. The only
		/// difference is whether an error gets generated.
		private bool _requiresContext = true;
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

		/// A flag for allowing the View to register with the Context
		/// In general you can ignore this. But some developers have asked for a way of disabling
		///  View registration with a checkbox from Unity, so here it is.
		/// If you want to expose this capability either
		/// (1) uncomment the commented-out line immediately below, or
		/// (2) subclass View and override the autoRegisterWithContext method using your own custom (public) field.
		//[SerializeField]
		protected bool registerWithContext = true;
		virtual public bool autoRegisterWithContext
		{
			get { return registerWithContext;  }
			set { registerWithContext = value; }
		}

		public bool registeredWithContext{get; set;}

		/// A MonoBehaviour Awake handler.
		/// The View will attempt to connect to the Context at this moment.
		protected virtual void Awake ()
		{
			if (autoRegisterWithContext && !registeredWithContext)
				bubbleToContext(this, true, false);
		}

		/// A MonoBehaviour Start handler
		/// If the View is not yet registered with the Context, it will 
		/// attempt to connect again at this moment.
		protected virtual void Start ()
		{
			if (autoRegisterWithContext && !registeredWithContext)
				bubbleToContext(this, true, true);
		}

		/// A MonoBehaviour OnDestroy handler
		/// The View will inform the Context that it is about to be
		/// destroyed.
		protected virtual void OnDestroy ()
		{
			bubbleToContext(this, false, false);
		}

		/// Recurses through Transform.parent to find the GameObject to which ContextView is attached
		/// Has a loop limit of 100 levels.
		/// By default, raises an Exception if no Context is found.
		virtual protected void bubbleToContext(MonoBehaviour view, bool toAdd, bool finalTry)
		{
			const int LOOP_MAX = 100;
			int loopLimiter = 0;
			Transform trans = view.gameObject.transform;
			while(trans.parent != null && loopLimiter < LOOP_MAX)
			{
				loopLimiter ++;
				trans = trans.parent;
				if (trans.gameObject.GetComponent<ContextView>() != null)
				{
					ContextView contextView = trans.gameObject.GetComponent<ContextView>() as ContextView;
					if (contextView.context != null)
					{
						IContext context = contextView.context;
						if (toAdd)
						{
							context.AddView(view);
							registeredWithContext = true;
							return;
						}
						else
						{
							context.RemoveView(view);
							return;
						}
					}
				}
			}
			if (requiresContext && finalTry)
			{
				//last ditch. If there's a Context anywhere, we'll use it!
				if (Context.firstContext != null)
				{
					Context.firstContext.AddView (view);
					registeredWithContext = true;
					return;
				}

				
				string msg = (loopLimiter == LOOP_MAX) ?
					msg = "A view couldn't find a context. Loop limit reached." :
						msg = "A view was added with no context. Views must be added into the hierarchy of their ContextView lest all hell break loose.";
				msg += "\nView: " + view.ToString();
				throw new MediationException(msg,
					MediationExceptionType.NO_CONTEXT);
			}
		}
	}
}

