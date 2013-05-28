using System;
using babel.extensions.mediation.api;
using babel.framework.impl;

namespace babel.extensions.mediation.impl
{
	public class MediationBinding : Binding, IMediationBinding
	{
		public MediationBinding (Binder.BindingResolver resolver) : base(resolver)
		{
		}

	}
}

