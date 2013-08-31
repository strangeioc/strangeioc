using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace strange.extensions.signal.api
{
    public interface IBaseSignal
    {
        void Dispatch(object[] args);
        void AddListener(Action<IBaseSignal, object[]> callback);
        void RemoveListener(Action<IBaseSignal, object[]> callback);
        List<Type> GetTypes();
    }
}
