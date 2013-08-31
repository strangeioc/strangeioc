using strange.extensions.injector.api;

public interface ICrossContextInjectionBinder : IInjectionBinder
{
    //Cross context Injection Binder is shared across all children
    IInjectionBinder CrossContextBinder { get; set; }

}
