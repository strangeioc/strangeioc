/**
 * @class strange.extensions.mediation.impl.Mediator
 * 
 * Base class for all Mediators.
 * 
 * @see strange.extensions.mediation.api.IMediationBinder
 */

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.api;

namespace strange.extensions.mediation.impl
{
	public class Mediator : MonoBehaviour, IMediator
	{
		protected MonoBehaviour abstractView;

		protected string viewID;

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView{get;set;}

		public Mediator ()
		{
		}

		virtual public void setViewComponent(MonoBehaviour view)
		{
			abstractView = view;
		}

		/**
		 * Fires directly after creation and before injection
		 */
		virtual public void preRegister()
		{
		}

		/**
		 * Fires after all injections satisifed.
		 *
		 * Override and place your initialization code here.
		 */
		virtual public void onRegister()
		{
		}

		/**
		 * Fires on removal of view.
		 *
		 * Override and place your cleanup code here
		 */
		virtual public void onRemove()
		{
		}
	}
}

