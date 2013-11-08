using System;

/// <summary>
/// Tag anything to be injected.
/// No arguments will bind a concrete class
/// Passing an Interface will bind to that interface
/// </summary>
[AttributeUsage(AttributeTargets.Class, 
                AllowMultiple = true,
                Inherited = true)]
public class DefaultImpl: Attribute
{
    public DefaultImpl() {  }
    public DefaultImpl(object n) { Name = n; }
    public DefaultImpl(Type t) { DefaultInterface = t; }
    public DefaultImpl(Type t, object n) { Name = n; }
	
	public object Name {get; set;}
    public Type DefaultInterface { get; set; }
}

/// <summary>
/// Tag an Interface with the Default Implementation Type
/// </summary>
[AttributeUsage(AttributeTargets.Interface,
                AllowMultiple = false,
                Inherited = true)]
public class ImplementedBy : Attribute
{
    public ImplementedBy(Type t) { DefaultType = t; }
    public Type DefaultType { get; set; }
}

/// <summary>
/// Tag an injectable to be a CrossContext Injection
/// </summary>
[AttributeUsage(AttributeTargets.Class,
                AllowMultiple = false,
                Inherited = true)]
public class CrossContextComponent : Attribute
{
    public CrossContextComponent() { }
}


/// <summary>
/// Tag a View class with the Mediator type used to mediate
/// </summary>
[AttributeUsage(AttributeTargets.Class,
                AllowMultiple = false,
                Inherited = true)]
public class Mediated : Attribute
{
    /// <summary>
    /// Tag a View class with the Mediator type used to mediate
    /// </summary>
    /// <param name="t">Mediator Type</param>
    public Mediated(Type t) { MediatorType = t; }

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

