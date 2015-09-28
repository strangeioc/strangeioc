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
using strange.framework.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.listBind.api
{
    public interface IListBinding : IBinding
    {
        new IListItemBinding To<T>();
        IListItemBinding ToValue(object o);

        IListBinding ToSingleton();

        Type ListType { get; set; }

        List<IListItemBinding> Bindings { get; }
    }
}
