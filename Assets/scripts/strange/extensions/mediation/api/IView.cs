/**
 * @class strange.extensions.mediation.api.IView
 * 
 * Monobehaviours must implement this interface in order to be injectable.
 * 
 * To contact the Context, the View must be able to find it. View handles this
 * with bubbling.
 */

using System;

namespace strange.extensions.mediation.api
{
	public interface IView
	{
		/// Indicates whether the View can work absent a context
		/// 
		/// Leave this value true most of the time. If for some reason you want
		/// a view to exist outside a context you can set it to false. The only
		/// difference is whether an error gets generated.
		bool requiresContext{ get; set;}
		
		/// Indicates whether this View  has been registered with a Context
		bool registeredWithContext{get; set;}
	}
}

