using UnityEngine;

namespace babel.extensions.mediation.impl
{
	public class View : MonoBehaviour
	{

		public View ()
		{
		}

		protected virtual void Awake ()
		{
			MediationBeacon.OnAwakeView (this);
		}

		protected virtual void OnEnable()
		{
			MediationBeacon.OnEnableView (this);
		}

		protected virtual void OnDisable()
		{
			MediationBeacon.OnDisableView(this);
		}

		protected virtual void OnDestroy ()
		{
			MediationBeacon.OnDestroyView (this);
		}
	}
}

