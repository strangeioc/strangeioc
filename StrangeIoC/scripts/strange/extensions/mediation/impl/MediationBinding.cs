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
 * MediationBindings support the following extensions of standard Bindings:
 *
 * ToMediator - Porcelain for To<T> providing a little extra clarity and security.
 *
 * ToAbstraction<T> - Provide an Interface or base Class adapter for the View.
 * When the binding specifies ToAbstraction<T>, the Mediator will be expected to inject <T>
 * instead of the concrete View class.
 */

using System;
using strange.extensions.mediation.api;
using strange.framework.impl;
using strange.framework.api;

namespace strange.extensions.mediation.impl
{
	public class MediationBinding : Binding, IMediationBinding
	{
		protected ISemiBinding _abstraction;


		public MediationBinding (Binder.BindingResolver resolver) : base(resolver)
		{
			_abstraction = new SemiBinding ();
			_abstraction.constraint = BindingConstraintType.ONE;
		}

		IMediationBinding IMediationBinding.ToMediator<T> ()
		{
			return base.To (typeof(T)) as IMediationBinding;
		}

		IMediationBinding IMediationBinding.ToAbstraction<T> ()
		{
			return ((IMediationBinding)this).ToAbstraction(typeof (T));
		}

		IMediationBinding IMediationBinding.ToAbstraction (Type t)
		{
			Type abstractionType = t;
			if (key != null)
			{
				Type keyType = key as Type;
				if (abstractionType.IsAssignableFrom(keyType) == false)
					throw new MediationException ("The View " + key.ToString() + " has been bound to the abstraction " + t.ToString() + " which the View neither extends nor implements. " , MediationExceptionType.VIEW_NOT_ASSIGNABLE);
			}
			_abstraction.Add (abstractionType);
			return this;
		}

		public object abstraction
		{ 
			get
			{
				return (_abstraction.value == null) ? BindingConst.NULLOID : _abstraction.value;
			}
		}

		new public IMediationBinding Bind<T>()
		{
			return base.Bind<T> () as IMediationBinding;
		}

		new public IMediationBinding Bind(object key)
		{
			return base.Bind (key) as IMediationBinding;
		}

		new public IMediationBinding To<T>()
		{
			return base.To<T> () as IMediationBinding;
		}

		new public IMediationBinding To(object o)
		{
			return base.To (o) as IMediationBinding;
		}
	}
}

