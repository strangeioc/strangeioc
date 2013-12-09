using System;
using strange.extensions.injector.api;

/// <summary>
/// Tag anything to be injected.
/// No arguments will bind a concrete class
/// Passing an Interface will bind to that interface
/// If an interface defines another defaulttype, this will override 
/// </summary>
[AttributeUsage(AttributeTargets.Class, 
                AllowMultiple = true,
                Inherited = true)]
public class Implements: Attribute
{
    public Implements() {  }
    public Implements(object name) { Name = name; }
    public Implements(Type t, InjectionBindingScope scope = InjectionBindingScope.SINGLE_CONTEXT)
    {
        DefaultInterface = t;
        Scope = scope;
    }

    public Implements(Type t, object name, InjectionBindingScope scope = InjectionBindingScope.SINGLE_CONTEXT)
    {
        DefaultInterface = t;
        Name = name;
        Scope = scope;
    }
    
    public Implements(InjectionBindingScope scope) { Scope = scope; }
    public Implements(InjectionBindingScope scope, object name)
    {
        Scope = scope;
        Name = name;
    }
	
	public object Name {get; set;}
    public Type DefaultInterface { get; set; }
    public InjectionBindingScope Scope { get; set; }
}

/// <summary>
/// Tag an Interface with the Default Implementation Type
/// If an Implements tag exists for this interface, it will override this default
/// </summary>
[AttributeUsage(AttributeTargets.Interface,
                AllowMultiple = false,
                Inherited = true)]
public class ImplementedBy : Attribute
{
    public ImplementedBy(Type t, InjectionBindingScope scope = InjectionBindingScope.SINGLE_CONTEXT)
    {
        DefaultType = t;
        Scope = scope;
    }

    public Type DefaultType { get; set; }
    public InjectionBindingScope Scope { get; set; }
}

/// <summary>
/// Tag a View class with the Mediator type used to mediate
/// </summary>
[AttributeUsage(AttributeTargets.Class,
                AllowMultiple = true,
                Inherited = true)]
public class MediatedBy : Attribute
{
    /// <summary>
    /// Tag a View class with the Mediator type used to mediate
    /// </summary>
    /// <param name="t">Mediator Type</param>
    public MediatedBy(Type t) { MediatorType = t; }

    public Type MediatorType { get; set; }
}

[AttributeUsage(AttributeTargets.Class,
                AllowMultiple = false,
                Inherited = true)]
public class Mediates : Attribute
{
    /// <summary>
    /// Tag a Mediator class with the View type it will be Mediating
    /// </summary>
    /// <param name="t"> View Type</param>
    public Mediates(Type t) { ViewType = t; }

    public Type ViewType { get; set; }
}

