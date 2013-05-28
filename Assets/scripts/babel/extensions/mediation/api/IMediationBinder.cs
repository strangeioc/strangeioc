using System;
using UnityEngine;
using babel.framework.api;

namespace babel.extensions.mediation.api
{
	public interface IMediationBinder : IBinder
	{
		void Trigger (MediationEvent evt, MonoBehaviour view);
	}
}

