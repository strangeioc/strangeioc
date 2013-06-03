/**
 * @interface strange.extensions.mediation.api.IMediator
 * 
 * Look at strange.extensions.mediation.api.IMediationBinder,
 * where I explain the purpose of Mediation in detail.
 * 
 * @see strange.extensions.mediation.api.IMediationBinder
 */

using System;
using UnityEngine;

namespace strange.extensions.mediation.api
{
	public interface IMediator
	{
		/// Get/set the GameObject that represents the top-most item in this Context
		GameObject contextView {get;set;}

		/// This method fires immediately after instantiation, but before injection.
		/// Override to handle anything that needs to happen prior to injection.
		void preRegister();

		/// This method fires immediately after injection.
		/// Override to perform the actions you might normally perform in a constructor.
		void onRegister();

		/// This method fires just before a GameObject will be destroyed.
		/// Override to clean up any listeners, or anything else that might keep the View/Mediator pair from being garbage collected.
		void onRemove();

		/// Sets the View.
		void setViewComponent(MonoBehaviour view);
	}
}

