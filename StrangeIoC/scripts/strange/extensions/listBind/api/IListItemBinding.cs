using strange.extensions.injector.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.listBind.api
{
    public interface IListItemBinding
    {
        object Resolve(IInjectionBinder injectionBinder);

        void ToSingleton();
    }
}
