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
using strange.framework.api;
using strange.framework.impl;
using System;
using System.Collections.Generic;

namespace strange.extensions.listBind.impl
{
    public class ListBinder : Binder, IListBinder
    {
        private readonly Dictionary<Type, IListBinding> listTypes = new Dictionary<Type, IListBinding>();
        private IInjectionBinder _injectionBinder;

        [Inject]
        public IInjectionBinder injectionBinder 
        {
            get
            {
                return this._injectionBinder;
            }
            set
            {
                this._injectionBinder = value;
                _injectionBinder.injector.listBinder = this;
            }
        }


        public IListBinding GetListBinding(Type type)
        {
            if (!listTypes.ContainsKey(type))
            {
                return null;
            }
            return listTypes[type];
        }

        public override IBinding GetRawBinding()
        {
            return new ListBinding(resolver, _injectionBinder) as IBinding;
        }

        public new IListBinding Bind<T>()
        {
            IListBinding binding = null;
            if (listTypes.ContainsKey(typeof(T)))
            {
                binding = listTypes[typeof(T)];   
            }
            else
            {
                binding = base.Bind<T>() as IListBinding;
                binding.ListType = typeof(IList<T>);
                listTypes[typeof(T)] = binding;
                injectionBinder.Bind<IList<T>>().To<List<T>>();
            }
            return binding;
        }
    }
}
