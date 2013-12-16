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
 * @class strange.extensions.injector.impl.Injector
 * 
 * Supplies injection for all mapped dependencies. 
 * 
 * Extension satisfies injection dependencies. Works in conjuntion with 
 * (and therefore relies on) the Reflector.
 * 
 * Dependencies may be Constructor injections (all parameters will be satisfied),
 * or setter injections.
 * 
 * Classes utilizing this injector must be marked with the following metatags:
 * <ul>
 *  <li>[Inject] - Use this metatag on any setter you wish to have supplied by injection.</li>
 *  <li>[Construct] - Use this metatag on the specific Constructor you wish to inject into when using Constructor injection. If you omit this tag, the Constructor with the shortest list of dependencies will be selected automatically.</li>
 *  <li>[PostConstruct] - Use this metatag on any method(s) you wish to fire directly after dependencies are supplied</li>
 * </ul>
 * 
 * The Injection system is quite loud and specific where dependencies are unmapped,
 * throwing Exceptions to warn you. This is exceptionally useful in ensuring that
 * your app is well structured.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using strange.extensions.injector.api;
using strange.extensions.reflector.api;

namespace strange.extensions.injector.impl
{
	public class Injector : IInjector
	{
		private Dictionary<IInjectionBinding, int> infinityLock;
		private const int INFINITY_LIMIT = 10;
		
		public Injector ()
		{
			factory = new InjectorFactory();
		}

		public IInjectorFactory factory{ get; set;}
		public IInjectionBinder binder{ get; set;}
		public IReflectionBinder reflector{ get; set;}

		public object Instantiate(IInjectionBinding binding)
		{
			failIf(binder == null, "Attempt to instantiate from Injector without a Binder", InjectionExceptionType.NO_BINDER);
			failIf(factory == null, "Attempt to inject into Injector without a Factory", InjectionExceptionType.NO_FACTORY);

			armorAgainstInfiniteLoops (binding);

			object retv = null;
			Type reflectionType = null;

			if (binding.value is Type)
			{
				reflectionType = binding.value as Type;
			}
			else if (binding.value == null)
			{
				object[] tl = binding.key as object[];
				reflectionType = tl [0] as Type;
				if (reflectionType.IsPrimitive || reflectionType == typeof(Decimal) || reflectionType == typeof(string))
				{
					retv = binding.value;
				}
			}
			else
			{
				retv = binding.value;
			}

			if (retv == null) //If we don't have an existing value, go ahead and create one.
			{
				
				IReflectedClass reflection = reflector.Get (reflectionType);

				Type[] parameters = reflection.constructorParameters;
				int aa = parameters.Length;
				object[] args = new object [aa];
				for (int a = 0; a < aa; a++)
				{
					args [a] = getValueInjection (parameters[a] as Type, null, null);
				}
				retv = factory.Get (binding, args);

				//If the InjectorFactory returns null, just return it. Otherwise inject the retv if it needs it
				//This could happen if Activator.CreateInstance returns null
				if (retv != null) 
				{
					if (binding.toInject)
					{
						retv = Inject (retv, false);
					}

					if (binding.type == InjectionBindingType.SINGLETON || binding.type == InjectionBindingType.VALUE)
					{
						//prevent double-injection
						binding.ToInject(false);
					}
				}
			}
			infinityLock = null; //Clear our infinity lock so the next time we instantiate we don't consider this a circular dependency

			return retv;
		}

		public object Inject(object target)
		{
			return Inject (target, true);
		}

		public object Inject(object target, bool attemptConstructorInjection)
		{
			failIf(binder == null, "Attempt to inject into Injector without a Binder", InjectionExceptionType.NO_BINDER);
			failIf(reflector == null, "Attempt to inject without a reflector", InjectionExceptionType.NO_REFLECTOR);
			failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

			//Some things can't be injected into. Bail out.
			Type t = target.GetType ();
			if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(string))
			{
				return target;
			}

			IReflectedClass reflection = reflector.Get (t);

			if (attemptConstructorInjection)
			{
				target = performConstructorInjection(target, reflection);
			}
			performSetterInjection(target, reflection);
			postInject(target, reflection);
			return target;
		}

		public void Uninject(object target)
		{
			failIf(binder == null, "Attempt to inject into Injector without a Binder", InjectionExceptionType.NO_BINDER);
			failIf(reflector == null, "Attempt to inject without a reflector", InjectionExceptionType.NO_REFLECTOR);
			failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

			Type t = target.GetType ();
			if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(string))
			{
				return;
			}

			IReflectedClass reflection = reflector.Get (t);

			performUninjection (target, reflection);
		}

		private object performConstructorInjection(object target, IReflectedClass reflection)
		{
			failIf(target == null, "Attempt to perform constructor injection into a null object", InjectionExceptionType.NULL_TARGET);
			failIf(reflection == null, "Attempt to perform constructor injection without a reflection", InjectionExceptionType.NULL_REFLECTION);

			ConstructorInfo constructor = reflection.constructor;
			failIf(constructor == null, "Attempt to construction inject a null constructor", InjectionExceptionType.NULL_CONSTRUCTOR);

			Type[] constructorParameters = reflection.constructorParameters;
			object[] values = new object[constructorParameters.Length];
			int i = 0;
			foreach (Type type in constructorParameters)
			{
				values[i] = getValueInjection(type, null, target);
				i++;
			}
			if (values.Length == 0)
			{
				return target;
			}

			object constructedObj = constructor.Invoke (values);
			return (constructedObj == null) ? target : constructedObj;
		}

		private void performSetterInjection(object target, IReflectedClass reflection)
		{
			failIf(target == null, "Attempt to inject into a null object", InjectionExceptionType.NULL_TARGET);
			failIf(reflection == null, "Attempt to inject without a reflection", InjectionExceptionType.NULL_REFLECTION);
			failIf(reflection.setters.Length != reflection.setterNames.Length, "Attempt to perform setter injection with mismatched names.\nThere must be exactly as many names as setters.", InjectionExceptionType.SETTER_NAME_MISMATCH);

			int aa = reflection.setters.Length;
			for(int a = 0; a < aa; a++)
			{
				KeyValuePair<Type, PropertyInfo> pair = reflection.setters [a];
				object value = getValueInjection(pair.Key, reflection.setterNames[a], target);
				injectValueIntoPoint (value, target, pair.Value);
			}
		}

		private object getValueInjection(Type t, object name, object target)
		{
			IInjectionBinding binding = binder.GetBinding (t, name);
			failIf(binding == null, "Attempt to Instantiate a null binding.", InjectionExceptionType.NULL_BINDING, t, name, target);
			if (binding.type == InjectionBindingType.VALUE)
			{
				if (!binding.toInject)
				{
					return binding.value;
				} else {
					object retv = Inject (binding.value, false);
					binding.ToInject (false);
					return retv;
				}
			} 
			else if (binding.type == InjectionBindingType.SINGLETON)
			{
				if (binding.value is Type || binding.value == null)
					Instantiate (binding);
				return binding.value;
			}
			else
			{
				return Instantiate (binding);
			}
		}

		//Inject the value into the target at the specified injection point
		private void injectValueIntoPoint(object value, object target, PropertyInfo point)
		{
			failIf(target == null, "Attempt to inject into a null target", InjectionExceptionType.NULL_TARGET);
			failIf(point == null, "Attempt to inject into a null point", InjectionExceptionType.NULL_INJECTION_POINT);
			failIf(value == null, "Attempt to inject null into a target object", InjectionExceptionType.NULL_VALUE_INJECTION);

			point.SetValue (target, value, null);
		}

		//After injection, call any methods labelled with the [PostConstruct] tag
		private void postInject(object target, IReflectedClass reflection)
		{
			failIf(target == null, "Attempt to PostConstruct a null target", InjectionExceptionType.NULL_TARGET);
			failIf(reflection == null, "Attempt to PostConstruct without a reflection", InjectionExceptionType.NULL_REFLECTION);

			MethodInfo[] postConstructors = reflection.postConstructors;
			if (postConstructors != null)
			{
				foreach(MethodInfo method in postConstructors)
				{
					method.Invoke (target, null);
				}
			}
		}

		//Note that uninjection can only clean publicly settable points
		private void performUninjection(object target, IReflectedClass reflection)
		{
			int aa = reflection.setters.Length;
			for(int a = 0; a < aa; a++)
			{
				KeyValuePair<Type, PropertyInfo> pair = reflection.setters [a];
				pair.Value.SetValue (target, null, null);
			}
		}

		private void failIf(bool condition, string message, InjectionExceptionType type)
		{
			failIf (condition, message, type, null, null, null);
		}

		private void failIf(bool condition, string message, InjectionExceptionType type, Type t, object name)
		{
			failIf(condition, message, type, t, name, null);
		}

		private void failIf(bool condition, string message, InjectionExceptionType type, Type t, object name, object target)
		{
			if (condition)
			{
				message += "\n\t\ttarget: " + target;
				message += "\n\t\ttype: " + t;
				message += "\n\t\tname: " + name;
				throw new InjectionException(message, type);
			}
		}

		private void armorAgainstInfiniteLoops(IInjectionBinding binding)
		{
			if (binding == null)
			{
				return;
			}
			if (infinityLock == null)
			{
				infinityLock = new Dictionary<IInjectionBinding, int> ();
			}
			if(infinityLock.ContainsKey(binding) == false)
			{
				infinityLock.Add (binding, 0);
			}
			infinityLock [binding] = infinityLock [binding] + 1;
			if (infinityLock [binding] > INFINITY_LIMIT)
			{
				throw new InjectionException ("There appears to be a circular dependency. Terminating loop.", InjectionExceptionType.CIRCULAR_DEPENDENCY);
			}
		}
	}
}

