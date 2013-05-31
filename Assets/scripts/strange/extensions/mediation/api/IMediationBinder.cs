using System;
using UnityEngine;
using strange.framework.api;

namespace strange.extensions.mediation.api
{
	public interface IMediationBinder : IBinder
	{
		void Trigger (MediationEvent evt, MonoBehaviour view);
	}
}

