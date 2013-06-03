/**
 * @interface strange.extensions.context.api.IContextView
 * 
 * The ContextView is the entry point to the application.
 * 
 * In a standard MVCSContext setup for Unity3D, it is a MonoBehaviour
 * attached to a GameObject at the very top of of your application.
 * It's most important action is to instantiate and call `Start()` on the Context.
 */

using System;

namespace strange.extensions.context.api
{
	public interface IContextView
	{
		/// Get and set the Context
		IContext context{get;set;}
	}
}

