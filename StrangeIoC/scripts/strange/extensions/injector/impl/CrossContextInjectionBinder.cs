﻿/*
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
 * @class strange.extensions.injector.impl.CrossContextInjectionBinder
 * 
 * A special version of InjectionBinder that allows shared injections across multiple Contexts.
 * 
 * @see strange.extensions.injector.api.IInjectionBinder
 * @see strange.extensions.injector.api.ICrossContextInjectionBinder
 */

using strange.extensions.injector.api;
using strange.framework.api;

namespace strange.extensions.injector.impl
{
	public class CrossContextInjectionBinder : InjectionBinder, ICrossContextInjectionBinder
	{
		public IInjectionBinder CrossContextBinder { get; set; }

		public CrossContextInjectionBinder() : base()
		{
		}

		public override IInjectionBinding GetBinding<T>()
		{
			return GetBinding(typeof(T), null);
		}


		public override IInjectionBinding GetBinding<T>(object name)//without this override Binder.GetBinding(object,object) gets called instead of CrossContextInjectionBinder.GetBind
		{
			return GetBinding(typeof(T), name);
		}

		public override IInjectionBinding GetBinding(object key)//without this override Binder.GetBinding(object,object) gets called instead of CrossContextInjectionBinder.GetBinding(
		{
			return GetBinding(key,null);
		}


		public override IInjectionBinding GetBinding(object key, object name)
		{
			IInjectionBinding binding = base.GetBinding(key, name) as IInjectionBinding;
			if (binding == null) //Attempt to get this from the cross context. Cross context is always SECOND PRIORITY. Local injections always override
			{
				if (CrossContextBinder != null)
				{
					binding = CrossContextBinder.GetBinding(key, name) as IInjectionBinding;
				}
			}
			return binding;
		}

		override public void ResolveBinding(IBinding binding, object key)
		{
			//Decide whether to resolve locally or not
			if (binding is IInjectionBinding)
			{
				InjectionBinding injectionBinding = (InjectionBinding)binding;
				if (injectionBinding.isCrossContext)
				{
					if (CrossContextBinder == null) //We are a crosscontextbinder
					{

						base.ResolveBinding(binding, key);
					}
					else 
					{
						base.Unbind(key, binding.name); //remove this cross context binding from ONLY the local binder
						CrossContextBinder.ResolveBinding(binding, key);
					}
				}
				else
				{
					base.ResolveBinding(binding, key);
				}
			}
		}

		protected override IInjector GetInjectorForBinding(IInjectionBinding binding)
		{
			if (binding.isCrossContext && CrossContextBinder != null)
			{
				return CrossContextBinder.injector;
			}
			else
			{
				return injector;
			}
		}

		public override void Unbind(object key, object name)
		{
			IInjectionBinding binding = GetBinding(key, name);

			if (binding != null && 
				binding.isCrossContext && 
				CrossContextBinder != null)
			{
				CrossContextBinder.Unbind(key, name);
			}

			base.Unbind(key, name);
		}
	}
}
