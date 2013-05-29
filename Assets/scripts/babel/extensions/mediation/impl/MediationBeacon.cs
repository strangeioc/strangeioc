/// Mediation beacon
/// =============================
/// Views can contact the static methods of this class to inform any interested
/// listeners of critical events in their lifecycle.
/// 
/// The preeminent expected use is for the creation/destruction of Mediators.

using System;
using UnityEngine;
using babel.extensions.mediation.api;
using babel.framework.api;
using babel.framework.impl;

namespace babel.extensions.mediation.impl
{
	public class MediationBeacon : IMediationBeacon
	{
		private static IMediationBinder _binder;
		private static ISemiBinding viewCache = new SemiBinding();
		
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
			else
				cacheView(view);
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
		
		public static void MediateCache()
		{
			if (_binder == null)
				return;
			
			object[] values = viewCache.value as object[];
			int aa = values.Length;
			for (int a = 0; a < aa; a++)
			{
				_binder.Trigger(MediationEvent.AWAKE, values[a] as MonoBehaviour);
			}
			viewCache = new SemiBinding();
		}
		
		private static void cacheView(MonoBehaviour view)
		{
			if (viewCache.constraint.Equals(BindingConstraintType.ONE))
			{
				viewCache.constraint = BindingConstraintType.MANY;
			}
			viewCache.Add(view);
		}
	}
}

