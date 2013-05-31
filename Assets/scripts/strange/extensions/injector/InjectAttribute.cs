using System;

//Tag [Inject] to create injections
[AttributeUsage(AttributeTargets.Property, 
                AllowMultiple = false,
                Inherited = true)]
public class Inject: Attribute
{
	public Inject(){}
	
	public Inject(object n)
	{
		name = n;
	}
	
	public object name{get; set;}
}

//Tag [PostConstruct] to perform post-injection construction actions
[AttributeUsage(AttributeTargets.Constructor, 
                AllowMultiple = false,
                Inherited = true)]
public class Construct: Attribute
{
	public Construct(){}
}

//Tag [PostConstruct] to perform post-injection construction actions
[AttributeUsage(AttributeTargets.Method, 
                AllowMultiple = false,
                Inherited = true)]
public class PostConstruct: Attribute
{
	public PostConstruct(){}
}

[AttributeUsage(AttributeTargets.Method, 
                AllowMultiple = false,
                Inherited = true)]
public class DeConstruct: Attribute
{
	public DeConstruct(){}
}