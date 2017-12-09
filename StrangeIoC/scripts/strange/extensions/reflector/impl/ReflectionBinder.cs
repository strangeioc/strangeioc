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
 * @class strange.extensions.reflector.impl.ReflectionBinder
 * 
 * Uses System.Reflection to create `ReflectedClass` instances.
 * 
 * Reflection is a slow process. This binder isolates the calls to System.Reflector 
 * and caches the result, meaning that Reflection is performed only once per class.
 */

using strange.extensions.reflector.api;
using strange.framework.api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace strange.extensions.reflector.impl
{
	public class ReflectionBinder : strange.framework.impl.Binder, IReflectionBinder
	{
		public ReflectionBinder ()
		{
		}

		public IReflectedClass Get<T> ()
		{
			return Get (typeof(T));
		}

		public IReflectedClass Get (Type type)
		{
			IBinding binding = GetBinding(type);
			IReflectedClass retv;
			if (binding == null)
			{
				binding = GetRawBinding ();
				IReflectedClass reflected = new ReflectedClass ();
				mapPreferredConstructor (reflected, binding, type);
				mapSetters (reflected, binding, type); //map setters before mapping methods
				mapMethods (reflected, binding, type); 
				binding.Bind (type).To (reflected);
				retv = binding.value as IReflectedClass;
				retv.PreGenerated = false;
			}
			else
			{
				retv = binding.value as IReflectedClass;
				retv.PreGenerated = true;
			}
			return retv;
		}

		public override IBinding GetRawBinding ()
		{
			IBinding binding = base.GetRawBinding ();
			binding.valueConstraint = BindingConstraintType.ONE;
			return binding;
		}

		private void mapPreferredConstructor(IReflectedClass reflected, IBinding binding, Type type)
		{
			ConstructorInfo constructor = findPreferredConstructor (type);
			if (constructor == null)
			{
				throw new ReflectionException("The reflector requires concrete classes.\nType " + type + " has no constructor. Is it an interface?", ReflectionExceptionType.CANNOT_REFLECT_INTERFACE);
			}
			ParameterInfo[] parameters = constructor.GetParameters();


			Type[] paramList = new Type[parameters.Length];
			object[] names = new object[parameters.Length];
			int i = 0;
			foreach (ParameterInfo param in parameters)
			{
				Type paramType = param.ParameterType;
				paramList [i] = paramType;

				object[] attributes = param.GetCustomAttributes(typeof(Name), false);
				if (attributes.Length > 0) 
				{
					names[i] = ( (Name)attributes[0]).name;
				}
				i++;
			}
			reflected.Constructor = constructor;
			reflected.ConstructorParameters = paramList;
			reflected.ConstructorParameterNames = names;
		}

		//Look for a constructor in the order:
		//1. Only one (just return it, since it's our only option)
		//2. Tagged with [Construct] tag
		//3. The constructor with the fewest parameters
		private ConstructorInfo findPreferredConstructor(Type type)
		{
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.FlattenHierarchy | 
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

		private void mapMethods(IReflectedClass reflected, IBinding binding, Type type)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.FlattenHierarchy | 
														 BindingFlags.Public |
														 BindingFlags.NonPublic |
														 BindingFlags.Instance |
														 BindingFlags.InvokeMethod);
			ArrayList methodList = new ArrayList ();
			List<KeyValuePair<MethodInfo, Attribute>> attrMethods = new List<KeyValuePair<MethodInfo, Attribute>>();
			foreach (MethodInfo method in methods)
			{
				object[] tagged = method.GetCustomAttributes (typeof(PostConstruct), true);
				if (tagged.Length > 0)
				{
					methodList.Add (method);
					attrMethods.Add(new KeyValuePair<MethodInfo, Attribute>(method, (Attribute) tagged[0]));
				}
				object[] listensToAttr = method.GetCustomAttributes(typeof (ListensTo), true);
				if (listensToAttr.Length > 0)
				{

					for (int i = 0; i < listensToAttr.Length; i++)
					{
						attrMethods.Add(new KeyValuePair<MethodInfo, Attribute>(method, (ListensTo) listensToAttr[i]));
					}
				}
			}

			methodList.Sort (new PriorityComparer ());
			reflected.postConstructors = (MethodInfo[])methodList.ToArray(typeof(MethodInfo));
			reflected.attrMethods = attrMethods.ToArray();
		}

		private void mapSetters(IReflectedClass reflected, IBinding binding, Type type)
		{
			MemberInfo[] privateMembers = type.FindMembers(MemberTypes.Property,
													BindingFlags.FlattenHierarchy |
													BindingFlags.SetProperty |
													BindingFlags.NonPublic |
													BindingFlags.Instance,
													null, null);
			foreach (MemberInfo member in privateMembers)
			{
				object[] injections = member.GetCustomAttributes(typeof(Inject), true);
				if (injections.Length > 0)
				{
					throw new ReflectionException ("The class " + type.Name + " has a non-public Injection setter " + member.Name + ". Make the setter public to allow injection.", ReflectionExceptionType.CANNOT_INJECT_INTO_NONPUBLIC_SETTER);
				}
			}

			MemberInfo[] members = type.FindMembers(MemberTypes.Property,
														  BindingFlags.FlattenHierarchy |
														  BindingFlags.SetProperty |
														  BindingFlags.Public |
														  BindingFlags.Instance,
														  null, null);

			//propertyinfo.name to reflectedattribute
			//This is to test for 'hidden' or overridden injections.
			Dictionary<String, ReflectedAttribute> namedAttributes = new Dictionary<string, ReflectedAttribute>();

			foreach (MemberInfo member in members)
			{
				object[] injections = member.GetCustomAttributes(typeof(Inject), true);
				if (injections.Length > 0)
				{
					Inject attr = injections [0] as Inject;
					PropertyInfo point = member as PropertyInfo;
					Type baseType = member.DeclaringType.BaseType;
					bool hasInheritedProperty = baseType != null ? baseType.GetProperties().Any(p => p.Name == point.Name) : false;
					bool toAddOrOverride = true; //add or override by default

					//if we have an overriding value, we need to know whether to override or leave it out.
					//We leave out the base if it's hidden
					//And we add if its overriding.
					if (namedAttributes.ContainsKey(point.Name))
						toAddOrOverride = hasInheritedProperty; //if this attribute has been 'hidden' by a new or override keyword, we should not add this.

					if (toAddOrOverride)
						namedAttributes[point.Name] = new ReflectedAttribute(point.PropertyType, point, attr.name);
				}
			}
			reflected.Setters = namedAttributes.Values.ToArray();
		}
	}

	class PriorityComparer : IComparer
	{
		int IComparer.Compare( Object x, Object y )
		{

			int pX = getPriority (x as MethodInfo);
			int pY = getPriority (y as MethodInfo);

			return (pX < pY) ? -1 : (pX == pY) ? 0 : 1;
		}

		private int getPriority(MethodInfo methodInfo)
		{
			PostConstruct attr = methodInfo.GetCustomAttributes(true) [0] as PostConstruct;
			int priority = attr.priority;
			return priority;
		}
	}
}

