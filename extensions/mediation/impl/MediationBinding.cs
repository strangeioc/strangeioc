/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

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

		IMediationBinding IMediationBinding.ToMediator<T> ()
		{
			return base.To (typeof(T)) as IMediationBinding;
		}
	}
}

