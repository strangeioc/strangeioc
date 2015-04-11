
using System;
using strange.extensions.signal.impl;

namespace strange.extensions.promise.api
{
    public interface IPromise
    {

        IPromise Progress(Action<float> listener);
        IPromise Fail(Action<Exception> listener);
        IPromise Finally(Action listener);

        void ReportFail(Exception ex);
        void ReportProgress(float progress);
        void RemoveAllListeners();
        void RemoveProgressListeners();
        void RemoveFailListeners();

        int ListenerCount();

        BasePromise.PromiseState State { get; }
    }
}
