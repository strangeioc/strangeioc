using System;

namespace strange.extensions.mediation.api
{
	public enum MediationEvent
	{
		/// The View is Awake
		AWAKE,

		/// The View is about to be Destroyed
		DESTROYED,

		/// The View is being Enabled
		ENABLED,

		/// The View is being Disabled
		DISABLED
	}
}

