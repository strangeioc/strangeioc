	using System;

[AttributeUsage(AttributeTargets.Method, 
		AllowMultiple = false,
		Inherited = true)]
public class ListensTo: Attribute
{
	public ListensTo(){}

	public ListensTo(Type t)
	{
		type = t;
	}
	
	public Type type {get; set;}
}
