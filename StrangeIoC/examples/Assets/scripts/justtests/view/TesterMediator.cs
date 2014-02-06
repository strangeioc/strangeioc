using System;
using strange.extensions.mediation.impl;

namespace strange.examples.justtests.view
{
	public class TesterMediator : Mediator
	{
		[Inject]
		public ITesterView view{ get; set; }


		override public void OnRegister()
		{
			view.Init ();
		}
	}
}

