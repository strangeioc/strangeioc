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
 * @interface strange.extensions.injector.api.IInjectionBinder
 * 
 * A Binder that implements Dependency Injection in StrangeIoC.
 * 
 * Keys in this Binder are always Types, that is, they represent
 * either Classes or Interfaces, not values. Values may be either Types
 * or values, depending on the situation.
 * 
 * The nature of the instance returned by `GetInstance()` depends on how
 * that Key was mapped.
 * 
 * examples:
 * 
 * //Returns a new instance of SimpleInterfaceImplementer.
 * 
 * `Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>();`
 * 
 * //Returns a Singleton instance of SimpleInterfaceImplementer.
 * 
 * `Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToSingleton();`
 * 
 * //Returns a Singleton instance of SimpleInterfaceImplementer.
 * 
 * `Bind<ISimpleInterface>().ToValue(new SimpleInterfaceImplementer());`
 * 
 * //Returns the value 42.
 * 
 * `Bind<int>().ToValue(42);`
 * 
 * //Returns a named instance of SimpleInterfaceImplementer for those whose
 * //injections specify this name. Note that once requested, this 
 * //same instance will be returned on any future request for that named instance.
 * 
 * `Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToName(SomeEnum.MY_ENUM);`
 * 
 * //Raises an Exception. string does not Implement ISimpleInterface.
 * 
 * `Bind<ISimpleInterface>().To<string>();`
 * 
 * @see strange.extensions.injector.api.IInjectionBinding
 */

using System;
using System.Collections.Generic;
using strange.framework.api;

namespace strange.extensions.injector.api
{
	public interface IInjectionBinder : IInstanceProvider
	{
		/// Get or set an Injector to use. By default, Injector instantiates it's own, but that can be overridden.
		IInjector injector{ get; set;}

		/// Retrieve an Instance based on a key/name combo.
		/// ex. `injectionBinder.Get(typeof(ISomeInterface), SomeEnum.MY_ENUM);`
		object GetInstance(Type key, object name);

		/// Retrieve an Instance based on a key/name combo.
		/// ex. `injectionBinder.Get<cISomeInterface>(SomeEnum.MY_ENUM);`
		T GetInstance<T>(object name);

		/// Reflect all the types in the list
		/// Return the number of types in the list, which should be equal to the list length
		int Reflect(List<Type> list);

		/// Reflect all the types currently registered with InjectionBinder
		/// Return the number of types reflected, which should be equal to the number
		/// of concrete classes you've mapped.
		int ReflectAll();

		/// <summary>
		/// Places individual Bindings into the bindings Dictionary as part of the resolving process
		/// </summary>
		/// Note that while some Bindings may store multiple keys, each key takes a unique position in the
		/// bindings Dictionary.
		/// 
		/// Conflicts in the course of fluent binding are expected, but GetBinding
		/// will throw an error if there are any unresolved conflicts.
		void ResolveBinding(IBinding binding, object key);

		IInjectionBinding Bind<T>();
		IInjectionBinding Bind(Type key);
		IBinding Bind(object key);
		IInjectionBinding GetBinding<T>();
		IInjectionBinding GetBinding<T>(object name);
		IInjectionBinding GetBinding(object key);
		IInjectionBinding GetBinding(object key, object name);
		void Unbind<T>();
		void Unbind<T>(object name);
		void Unbind (object key);
		void Unbind (object key, object name);
		void Unbind (IBinding binding);
	}
}

