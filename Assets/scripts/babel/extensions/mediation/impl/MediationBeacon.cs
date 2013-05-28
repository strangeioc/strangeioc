/// Mediation beacon
/// =============================
/// Views can contact the static methods of this class to inform any interested
/// listeners of critical events in their lifecycle.
/// 
/// The preeminent expected use is for the creation/destruction of Mediators.

using System;
using UnityEngine;
using babel.extensions.mediation.api;

namespace babel.extensions.mediation.impl
{
	public class MediationBeacon : IMediationBeacon
	{
		private static IMediationBinder _binder;
		
		
		[Inject]
		public IMediationBinder binder
		{
			get
			{
				return _binder;
			}
			set
			{
				_binder = value;
			}
		}

		public static void OnAwakeView(MonoBehaviour view)
		{
			if (_binder != null)
				_binder.Trigger (MediationEvent.AWAKE, view);
		}

		public static void OnDestroyView(MonoBehaviour view)
		{
			if (_binder != null)
				_binder.Trigger (MediationEvent.DESTROYED, view);
		}

		public static void OnEnableView(MonoBehaviour view)
		{
			if (_binder != null)
				_binder.Trigger (MediationEvent.ENABLED, view);
		}

		public static void OnDisableView(MonoBehaviour view)
		{
			if (_binder != null)
				_binder.Trigger (MediationEvent.DISABLED, view);
		}
	}
}

