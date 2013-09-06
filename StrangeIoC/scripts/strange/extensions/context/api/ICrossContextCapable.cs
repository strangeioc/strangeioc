using strange.extensions.dispatcher.api;

public interface ICrossContextCapable
{
    /// Add cross context functionality to a child context being added
    void AssignCrossContext(ICrossContextCapable childContext);
    /// Clean up cross context functionality to a child context being removed
    void RemoveCrossContext(ICrossContextCapable childContext);
    /// Request a component from the context (might be useful in certain cross-context situations)
    object GetComponent<T>();
    /// Request a component from the context (might be useful in certain cross-context situations)
    object GetComponent<T>(object name);
    
    ////Cross context Injection Binder is shared across all children
    //IInjectionBinder CrossContextInjectionBinder { get; set; }

    /// All cross context capable contexts must implement an injectionBinder
    ICrossContextInjectionBinder injectionBinder { get; set; }

    
    /// Set and get the shared system bus for communicating across contexts
    IDispatcher crossContextDispatcher { get; set; }



}