/// This Context is for development and testing.
/// ===============
/// If you're looking for stuff to get you started, try MyFirstProjectRoot and MyFirstContext.
/// I just use this file to try out experiments and stuff. Usually it's a good idea to ignore it.
using UnityEngine;
using strange.extensions.context.api;
using strange.examples.justtests.view;
using strange.extensions.context.impl;
using strange.extensions.mediation.api;



namespace strange.examples.justtests
{
	public class TesterContext : MVCSContext
	{
		public TesterContext (MonoBehaviour view) : base(view)
		{
		}

		public TesterContext (MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
		{
		}

		protected override void mapBindings()
		{
			//Bind some abstract and/or concrete Views


			Debug.LogWarning ("mapBindings: " + mediationBinder);

			IMediationBinding binding = mediationBinder.Bind<TesterView> ();

			Debug.LogWarning ("binding: " + binding);

			binding = binding.ToAbstraction<ITesterView> ();

			Debug.LogWarning ("binding2: " + binding);

			binding = binding.To<TesterMediator> ();

		}
	}
}

