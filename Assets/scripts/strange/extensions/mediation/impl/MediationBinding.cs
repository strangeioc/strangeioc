using System;
using strange.extensions.mediation.api;
using strange.framework.impl;

namespace strange.extensions.mediation.impl
{
	public class MediationBinding : Binding, IMediationBinding
	{
		public MediationBinding (Binder.BindingResolver resolver) : base(resolver)
		{
		}

	}
}

