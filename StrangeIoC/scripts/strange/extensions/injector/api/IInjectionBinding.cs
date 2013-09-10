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
 * @interface strange.extensions.injector.api.IInjectionBinding
 * 
 * The Binding form for the Injection system.
 * 
 * The InjectionBinding allows mapping to three core types:
 * <ul>
 *  <li>Default: every `GetInstance()` on the Binder Key returns a new imstance</li>
 *  <li>ToSingleton: every `GetInstance()` on the Binder Key returns the same imstance</li>
 *  <li>ToValue: every `GetInstance()` on the Binder Key returns the provided imstance</li>
 * </ul>
 * 
 * Named injections are supported, thus:
 * 
 		Bind<IMyInterface>().To<MyInterfaceImplementer>().ToName(SomeEnum.VALUE);
 * 
 * returns a MyInterfaceImplementer instance only to injections that specifically tag
 * SomeEnum.Value. This will be the same instance whenever it is called.
 * 
 * You can also map multiple Keys, allowing for polymorphic binding.
 * This allows you to match two or more interfaces to a single class or value.
 * 
 		Bind<IFirstInterface>().Bind<ISecondInterface>().To<PolymorphicClass>();
 * 
 * Note that while you can bind multiple keys to an InjectionBinding, you can
 * only bind one value. (The Injection system needs to know which concrete
 * class you want created.)
 * 
 * @see strange.extensions.injector.api.IInjectionBinder
 */

using System;

namespace strange.extensions.injector.api
{
	public interface IInjectionBinding
	{
		/// Map the Binding to a Singleton so that every `GetInstance()` on the Binder Key returns the same imstance.
		IInjectionBinding ToSingleton();

		/// Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided imstance. Sets type to Value
		IInjectionBinding ToValue (object o);

		/// Map the Binding to a stated instance so that every `GetInstance()` on the Binder Key returns the provided imstance. Does not set type.
		IInjectionBinding SetValue(object o);

		/// Map the binding and give access to all contexts in hierarchy
		IInjectionBinding CrossContext();

		bool isCrossContext { get; }

		/// Boolean setter to optionally override injection. If false, the instance will not be injected after instantiation.
		IInjectionBinding ToInject(bool value);

		/// Get the parameter that specifies whether this Binding allows an instance to be injected
		bool toInject{get;}

		/// Get and set the InjectionBindingType
		/// @see InjectionBindingType
		InjectionBindingType type{get; set;}

		/// Bind is the same as Key, but permits Binder syntax: `Bind<T>().Bind<T>()`
		IInjectionBinding Bind<T>();

		/// Bind is the same as Key, but permits Binder syntax: `Bind<T>().Bind<T>()`
		IInjectionBinding Bind(object key);

		IInjectionBinding Key<T>();
		IInjectionBinding Key(object key);
		IInjectionBinding To<T>();
		IInjectionBinding To(object o);
		IInjectionBinding ToName<T> ();
		IInjectionBinding ToName (object o);
		IInjectionBinding Named<T>();
		IInjectionBinding Named(object o);

		object key{ get; }
		object name{ get; }
		object value{ get; }
		Enum keyConstraint{ get; set;}
		Enum valueConstraint{ get; set;}
	}
}

