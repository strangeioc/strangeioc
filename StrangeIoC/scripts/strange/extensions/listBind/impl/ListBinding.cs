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
using System.Linq;
using System.Collections.Generic;
using strange.extensions.injector.impl;

namespace strange.extensions.listBind.impl
{
    public class ListBinding : Binding, IListBinding
    {
        private readonly List<IListItemBinding> bindings = new List<IListItemBinding>();

        private readonly IInjectionBinder injectionBinder;

        public ListBinding(Binder.BindingResolver resolver, IInjectionBinder injectionBinder)
            : base(resolver)
		{
            this.injectionBinder = injectionBinder;
		}

        public List<IListItemBinding> Bindings { get { return this.bindings; } }
        public Type ListType { get; set; }

        public Type ListItemType 
        {
            get
            {
                return ListType.GetGenericArguments()[0];
            }
        }

        public new IListItemBinding To<T>()
        {
            var injectionBinding = injectionBinder.GetBinding<T>();
            if (injectionBinding == null)
            {
                injectionBinding = new InjectionBinding(resolver);
                injectionBinding.Bind(typeof(T));
            }
        
            ToTypeBinding toType = new ToTypeBinding(injectionBinding, ListItemType, typeof(T));
            this.bindings.Add(toType);
            return toType;

        }

        public IListItemBinding ToValue(object o)
        {
            ToValueBinding toValue = new ToValueBinding(o);
            this.bindings.Add(toValue);
            return toValue;
        }


        
        public IListBinding ToSingleton()
        {
            var listBinding = injectionBinder.GetBinding(ListType);
            listBinding.ToSingleton();
            return this;
        }
         
    }
}
