/**
 * @interface strange.extensions.mediation.impl.MediationBinding
 * 
 * Subclass of Binding for MediationBinding.
 * 
 * I've provided MediationBinding, but at present it comforms
 * perfectly to Binding.
 */

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

