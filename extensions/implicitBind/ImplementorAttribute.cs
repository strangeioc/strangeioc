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

using System;
using strange.extensions.injector.api;

/// <summary>
/// Declares a Class to be implicitly bound.
/// No arguments binds a concrete class to itself
/// Passing an Interface binds to that interface
/// Implements overrides ImplementedBy
/// </summary>
[AttributeUsage(AttributeTargets.Class, 
				AllowMultiple = true,
				Inherited = true)]
public class Implements: Attribute
{
	/// <summary>
	/// Concretely bind to own type
	/// </summary>
	public Implements() {  }

	/// <summary>
	/// Concretely bind to own type and specify scope
	/// </summary>
	/// <param name="scope"></param>
	public Implements(InjectionBindingScope scope) { Scope = scope; }

	/// <summary>
	/// Bind to an interface, specify the scope if necessary
	/// </summary>
	/// <param name="t">Interface to bind to</param>
	/// <param name="scope">Single Context(default) or Cross Context.</param>
	public Implements(Type t, InjectionBindingScope scope = InjectionBindingScope.SINGLE_CONTEXT)
	{
		DefaultInterface = t;
		Scope = scope;
	}

	/// <summary>
	/// Bind concretely, specifying a scope and object name
	/// </summary>
	/// <param name="scope"></param>
	/// <param name="name"></param>
	public Implements(InjectionBindingScope scope, object name)
	{
		Scope = scope;
		Name = name;
	}
	/// <summary>
	/// Bind to an interface, specifying a scope and object name
	/// </summary>
	/// <param name="t">Interface to bind to</param>
	/// <param name="scope">Single Context or Cross Context</param>
	/// <param name="name">Name to bind to</param>
	public Implements(Type t, InjectionBindingScope scope, object name)
	{
		DefaultInterface = t;
		Name = name;
		Scope = scope;
	}
	
	public object Name {get; set;}
	public Type DefaultInterface { get; set; }
	public InjectionBindingScope Scope { get; set; }
}

/// <summary>
/// Declares an interface to have an implicit implementor
/// An Implements tag for the given interface overrides this tag.
/// </summary>
[AttributeUsage(AttributeTargets.Interface,
				AllowMultiple = false,
				Inherited = true)]
public class ImplementedBy : Attribute
{
	/// <summary>
	/// Bind this interface to a default type t
	/// </summary>
	/// <param name="t">Default Type</param>
	/// <param name="scope">Single Context(default) or Cross Context</param>
	public ImplementedBy(Type t, InjectionBindingScope scope = InjectionBindingScope.SINGLE_CONTEXT)
	{
		DefaultType = t;
		Scope = scope;
	}

	public Type DefaultType { get; set; }
	public InjectionBindingScope Scope { get; set; }
}

/// <summary>
/// Declares a View class implicity mediated by one or more named Mediators
/// </summary>
[AttributeUsage(AttributeTargets.Class,
				AllowMultiple = true,
				Inherited = true)]
public class MediatedBy : Attribute
{
	/// <summary>
	/// Bind this view to a Mediator
	/// </summary>
	/// <param name="t">Mediator Type</param>
	public MediatedBy(Type t) { MediatorType = t; }

	public Type MediatorType { get; set; }
}

/// <summary>
/// Declare a Mediator class implicitly bound to a provided View
/// </summary>
[AttributeUsage(AttributeTargets.Class,
				AllowMultiple = false,
				Inherited = true)]
public class Mediates : Attribute
{
	/// <summary>
	/// Bind this Mediator to a view
	/// </summary>
	/// <param name="t">View Type</param>
	public Mediates(Type t) { ViewType = t; }

	public Type ViewType { get; set; }
}

