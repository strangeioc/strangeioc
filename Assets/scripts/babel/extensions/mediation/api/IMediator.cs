using System;
using UnityEngine;

namespace babel.extensions.mediation.api
{
	public interface IMediator
	{
		GameObject contextView {get;set;}
		void preRegister();
		void onRegister();
		void onRemove();
		void setViewComponent(MonoBehaviour view);
	}
}

