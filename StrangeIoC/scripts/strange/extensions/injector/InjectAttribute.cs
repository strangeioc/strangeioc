/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

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


 * @class Name
 *  
 * When a parameter of a constructor or pseudo-constructor is tagged with [Name], 
 * the injector can discriminate between different classes that satisfy the same interface.  
 * This means that constructors and pseudo-constructors can used named injection just like 
 * setter injection.
 * 
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
 * You may optionally include a priority int on your PostConstructor. This allows for multiple
 * PostConstruction methods which will fire in a predictable order.
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

//Tag [Name] to perform named injection in constructors and pseudo-constructors
[AttributeUsage(AttributeTargets.Parameter,
        AllowMultiple = false,
        Inherited = false)]
public class Name : Attribute 
{
	public Name(object n) 
	{
		name = n;
	}

	public object name { get; set; }
}

//Tag [Construct] to perform construction injection
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

	public PostConstruct(int p)
	{
		priority = p;
	}

	public int priority{get; set;}
}

[AttributeUsage(AttributeTargets.Method, 
		AllowMultiple = false,
		Inherited = true)]
public class Deconstruct: Attribute
{
	public Deconstruct(){}
}