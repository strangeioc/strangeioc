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
 * @interface strange.extensions.mediation.api.IMediationBinding
 * 
 * Interface for MediationBindings
 * 
 * Adds porcelain method to clarify View/Mediator binding.
 */

using System;
using strange.framework.api;

namespace strange.extensions.mediation.api
{
	public interface IMediationBinding : IBinding
	{
		/// Porcelain for To<T> providing a little extra clarity and security.
		IMediationBinding ToMediator<T>();

		/// Provide an Interface or base Class adapter for the View.
		/// When the binding specifies ToAbstraction<T>, the Mediator will be expected to inject <T>
		/// instead of the concrete View class.
		IMediationBinding ToAbstraction<T>();

		IMediationBinding ToAbstraction(Type t);

		/// Retrieve the abstracted value set by ToAbstraction<T>
		object abstraction { get; }

		new IMediationBinding Bind<T>();
		new IMediationBinding Bind(object key);
		new IMediationBinding To<T>();
		new IMediationBinding To(object o);
	}
}

