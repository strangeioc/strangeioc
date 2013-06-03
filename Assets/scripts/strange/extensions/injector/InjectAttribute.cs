/**
 * @class Inject
 * 
 * The `[Inject]` attribute marks a setter Injection point.
 * 
 * Example:

		[Inject]
		public IMyInterface myInstance{get;set;}

 * 
 * Inject tags can also specify a name:
 * 

 		[Inject(SomeEnum.VALUE)]
 		public IMyInterface myInstance{get;set;}


 * @class Construct
 * 
 * The `[Construct]` attribute marks a preferred Constructor. If omitted,
 * the Reflector will mark as Constructor the shortest available
 * Constructor. Obviously, if there only one constructor, this tag
 * is not requried.
 * 
 * @class PostConstruct
 * 
 * The `[PostConstruct]` attribute marks one or more methods as PostConstructors.
 * A PostConstructor is triggered immediately after injection. This allows
 * you to use use a PostConstructor in much the same way as a Constructor,
 * safe in the knowledge that there will be no null pointers on injected
 * dependencies. PostConstructors do not accept arguments.
 * 
 * @class Deconstruct
 * 
 * Unsupported.
 */

using System;

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
public class Deconstruct: Attribute
{
	public Deconstruct(){}
}