using strange.extensions.injector.api;
using strange.framework.api;

public interface ICrossContextInjectionBinder : IInjectionBinder
{
    //Cross context Injection Binder is shared across all children
    IInjectionBinder CrossContextBinder { get; set; }

}
