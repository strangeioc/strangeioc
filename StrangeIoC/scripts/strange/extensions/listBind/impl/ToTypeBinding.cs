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

using strange.extensions.injector.api;
using strange.extensions.listBind.api;
using System;

namespace strange.extensions.listBind.impl
{
    public class ToTypeBinding : IListItemBinding
    {
        private readonly Type fromType;
        private readonly Type type;
        private bool asSingleton;
        private object instance;

        private IInjectionBinding injectionBinding;

        public ToTypeBinding(IInjectionBinding injectionBinding, Type fromType, Type type)
        {
            this.fromType = fromType;
            this.type = type;
            this.injectionBinding = injectionBinding;
            asSingleton = false;
        }

        public object Resolve(IInjectionBinder injectionBinder)
        {
            // First give a chance for the "officially" bound bindings to find the instances
            var bindingType = this.type;
            IInjectionBinding binding = injectionBinder.GetBinding(bindingType);
            if (binding == null)
            {
                binding = injectionBinder.GetBinding(this.fromType);
                bindingType = this.fromType;
            }
            if (binding != null)
            {
                var bound = injectionBinder.GetInstance(bindingType, null);
                if (bound.GetType().Equals(type))
                {
                    return bound;
                }
            }

            // Then, let's see if we have a singleton
            if (asSingleton && this.instance != null)
            {
                return this.instance;
            }

            // Then we'll use the temporary binding to create the instance.
            var instance = injectionBinder.injector.Instantiate(this.injectionBinding, false);
            if (asSingleton)
            {
                this.instance = instance;
            }
            return instance;

        }

        public void ToSingleton()
        {
            asSingleton = true;
        }

    }
}