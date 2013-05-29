/**
 * Extension which uses System.Reflection to satisfy injection dependencies.
 * 
 * Dependencies may be Constructor injections (all parameters will be satisfied),
 * or setter injections.
 * 
 * Classes utilizing this injector must be marked with the following metatags:
 * - [Inject]
 * Use this metatag on any setter you wish to have supplied by injection
 * - [Construct]
 * Use this metatag on the specific Constructor you wish to inject into when using Constructor injection
 * - [PostConstruct]
 * Use this metatag on any method(s) you wish to fire directly after dependencies are supplied
 * 
 * TODO: Reflection is innately inefficient. Can improve performace by caching class mappings
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using babel.framework.api;
using babel.extensions.injector.api;

namespace babel.extensions.injector.impl
{
	public class Injector : IInjector
	{
		private Dictionary<IInjectionBinding, int> infinityLock;
		private int INFINITY_LIMIT = 10;
		
		public Injector ()
		{
			factory = new InjectorFactory();
		}

		public IInjectorFactory factory{ get; set;}
		public IInjectionBinder binder{ get; set;}

		public object Instantiate(IInjectionBinding binding)
		{
			failIf(binder == null, "Attempt to instantiate from Injector without a Binder", InjectionExceptionType.NO_BINDER);
			failIf(factory == null, "Attempt to inject into Injector without a Factory", InjectionExceptionType.NO_FACTORY);

			armorAgainstInfiniteLoops (binding);

			object retv = factory.Get (binding);

			//Factory can return null in the case that there are no parameterless constructors.
			//In this case, the following routine attempts to generate based on a preferred constructor
			if (retv == null)
			{
				//Learn about constructors
				object[] keys = binding.key as object[];
				ConstructorInfo constructor = findPreferredConstructor (keys[0] as Type);
				//If no valid constructor, abort
				if (constructor == null)
				{
					return null;
				}
				//Collect params to satisfy constructor injection
				ParameterInfo[] parameters = constructor.GetParameters ();
				object[] args = new object [parameters.Length];
				int aa = parameters.Length;
				for (int a = 0; a < aa; a++)
				{
					ParameterInfo parameter = parameters [a];
					Type parameterType = parameter.ParameterType;
					IInjectionBinding parameterBinding = binder.GetBinding(parameterType);
					if (parameterBinding == null)
					{
						return null;
					}
					args [a] = Instantiate (parameterBinding);
				}
				//Pass back to factory and instantiate
				retv = factory.Get (binding, args);
				if (retv == null)
				{
					return null;
				}
				retv = Inject (retv, false);
			}
			else //could be more efficient. There are cases (VALUE) where we're re-injecting
			{
				retv = Inject (retv, true);
			}
			infinityLock = null;

			return retv;
		}

		public object Inject(object target)
		{
			return Inject (target, true);
		}

		protected object Inject(object target, bool attemptConstructorInjection)
		{
			failIf(binder == null, "Attempt to inject into Injector without a Binder", InjectionExceptionType.NO_BINDER);
			failIf(target == null, "Attempt to inject into null instance", InjectionExceptionType.NULL_TARGET);

			//Some things can't be injected into. Bail out.
			Type t = target.GetType ();
			if (t.IsPrimitive || t == typeof(Decimal) || t == typeof(string))
			{
				return target;
			}
			if (attemptConstructorInjection)
			{
				target = performConstructorInjection(target);
			}
			performPropertyInjection(target);
			postInject(target);
			return target;
		}


		//Look for a constructor in the order:
		//1. Only one (just return it, since it's our only option)
		//2. Tagged with [Construct] tag
		//3. The constructor with the fewest parameters
		private ConstructorInfo findPreferredConstructor(Type targetType)
		{
			ConstructorInfo[] constructors = targetType.GetConstructors(BindingFlags.FlattenHierarchy | 
			                                                            BindingFlags.Public | 
			                                                            BindingFlags.Instance |
			                                                            BindingFlags.InvokeMethod);
			if (constructors.Length == 1)
			{
				return constructors [0];
			}
			int len;
			int shortestLen = int.MaxValue;
			ConstructorInfo shortestConstructor = null;
			foreach (ConstructorInfo constructor in constructors)
			{
				object[] taggedConstructors = constructor.GetCustomAttributes(typeof(Construct), true);
				if (taggedConstructors.Length > 0)
				{
					return constructor;
				}
				len = constructor.GetParameters ().Length;
				if (len < shortestLen)
				{
					shortestLen = len;
					shortestConstructor = constructor;
				}
			}
			return shortestConstructor;
		}

		private object performConstructorInjection(object target)
		{
			failIf(target == null, "Attempt to inject into a null object", InjectionExceptionType.NULL_TARGET);

			Type targetType = target.GetType();
			ConstructorInfo constructor = findPreferredConstructor (targetType);
			failIf(constructor == null, "Attempt to construction inject a null constructor", InjectionExceptionType.NULL_CONSTRUCTOR);
			object constructedObj = performConstructorInjectionOn (constructor);
			return (constructedObj == null) ? target : constructedObj;
		}

		private object performConstructorInjectionOn(ConstructorInfo constructor)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			object[] paramList = new object[parameters.Length];
			int i = 0;
			foreach (ParameterInfo param in parameters)
			{
				Type paramType = param.ParameterType;
				paramList[i] = getValueInjection(paramType, null);
			}
			if (paramList.Length == 0) 
				return null;
			else
				return constructor.Invoke (paramList);
		}

		private object getValueInjection(Type t, object name)
		{
			IInjectionBinding binding = binder.GetBinding (t, name);
			failIf(binding == null, "Attempt to Instantiate a null binding.", InjectionExceptionType.NULL_BINDING, t, name);
			return Instantiate (binding);
		}

		//Inject the value into the target at the specified injection point
		private void injectValueIntoPoint(object value, object target, PropertyInfo point)
		{
			failIf(target == null, "Attempt to inject into a null target", InjectionExceptionType.NULL_TARGET);
			failIf(point == null, "Attempt to inject into a null point", InjectionExceptionType.NULL_INJECTION_POINT);
			failIf(value == null, "Attempt to inject null into a target object", InjectionExceptionType.NULL_VALUE_INJECTION);

			point.SetValue (target, value, null);
		}

		private void performPropertyInjection(object target)
		{
			failIf(target == null, "Attempt to inject into a null object", InjectionExceptionType.NULL_TARGET);
			Type targetType = target.GetType();
			MemberInfo[] members = targetType.FindMembers(MemberTypes.Property,
			                                            	BindingFlags.FlattenHierarchy | 
			                                            	BindingFlags.SetProperty | 
			                                            	BindingFlags.Public | 
			                                            	BindingFlags.Instance, 
			                                            	null, null);

			foreach (MemberInfo member in members)
			{
				object[] injections = member.GetCustomAttributes(typeof(Inject), true);
				foreach (Inject attr in injections)
				{
					object bindingName = attr.name;
					PropertyInfo point = member as PropertyInfo;
					Type pointType = point.PropertyType;
					object value = getValueInjection(pointType, bindingName);
					injectValueIntoPoint (value, target, point);
				}
			}
		}

		//After injection, call any methods labelled with the [PostConstruct] tag
		private void postInject(object target)
		{
			failIf(target == null, "Attempt to postInject a null target", InjectionExceptionType.NULL_TARGET);

			Type targetType = target.GetType();
			MethodInfo[] methods = targetType.GetMethods(BindingFlags.FlattenHierarchy | 
			                                             BindingFlags.Public | 
			                                             BindingFlags.Instance |
			                                             BindingFlags.InvokeMethod);
			foreach (MethodInfo method in methods)
			{
				object[] postConstructs = method.GetCustomAttributes(typeof(PostConstruct), true);
				if (postConstructs.Length > 0)
				{
					method.Invoke(target, null);
				}
			}
		}

		private void failIf(bool condition, string message, InjectionExceptionType type)
		{
			if (condition)
			{
				throw new InjectionException(message, type);
			}
		}

		private void failIf(bool condition, string message, InjectionExceptionType type, Type t, object name)
		{
			if (condition)
			{
				if (t != null)
				{
					message += "\n\t\ttype: " + t.ToString ();
				}
				if (name != null)
				{
					message += "\n\t\tname: " + name.ToString ();
				}
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

