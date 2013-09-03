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
 * @class strange.extensions.injector.impl.InjectorFactory
 * 
 * The Factory that instantiates all instances.
 */

using System;
using System.Collections.Generic;
using strange.extensions.injector.api;

namespace strange.extensions.injector.impl
{
	public class InjectorFactory : IInjectorFactory
	{
		protected Dictionary<IInjectionBinding, Dictionary<object, object>> objectMap;

		public InjectorFactory ()
		{
			objectMap = new Dictionary<IInjectionBinding, Dictionary<object, object>> ();
		}

		public object Get(IInjectionBinding binding, object[] args)
		{
            
			if (binding == null)
			{
				throw new InjectionException ("InjectorFactory cannot act on null binding", InjectionExceptionType.NULL_BINDING);
			}
			InjectionBindingType type = binding.type;
			if (objectMap.ContainsKey(binding) == false)
			{
				objectMap [binding] = new Dictionary<object, object> ();
			}

			switch (type)
			{
				case InjectionBindingType.SINGLETON:
					return singletonOf (binding, args);
				case InjectionBindingType.VALUE:
					return valueOf (binding);
				default:
					break;
			}
			return instanceOf (binding, args);
		}

		public object Get(IInjectionBinding binding)
		{
			return Get (binding, null);
		}

		/// Generate a Singleton instance
		protected object singletonOf(IInjectionBinding binding, object[] args)
		{
            
			object name = binding.name;
			Dictionary<object, object> dict = objectMap [binding];
            if (dict.ContainsKey(name) == false)
            {
                if (binding.value != null)
                {
                    dict[name] = createFromValue(binding.value, args);
                }
                else
                {
                    foreach (object objectMapKey in objectMap.Keys)
                    {
                        InjectionBinding storedBinding = (InjectionBinding)objectMapKey;
                    }
                    binding.ToValue(generateImplicit((binding.key as object[])[0], args));
                    dict[name] = binding.value;
                }
            }
            else
            {
                foreach (object objectMapKey in objectMap.Keys)
                {
                    InjectionBinding storedBinding = (InjectionBinding)objectMapKey;
                }
            }
			return dict[name];
		}

		protected object generateImplicit(object key, object[] args)
		{
			Type type = key as Type;
			if (!type.IsInterface && !type.IsAbstract)
			{
				return createFromValue(key, args);
			}
			throw new InjectionException ("You can't create a class that can't be instantiated. Class: " + key.ToString(), InjectionExceptionType.NOT_INSTANTIABLE);
		}

		/// The binding already has a value. Simply return it.
		protected object valueOf(IInjectionBinding binding)
		{
			return binding.value;
		}

		/// Generate a new instance
		protected object instanceOf(IInjectionBinding binding, object[] args)
		{
			if (binding.value != null)
			{
				return createFromValue(binding.value, args);
			}
			object value = generateImplicit ((binding.key as object[]) [0], args);
			return createFromValue(value, args);
		}

		/// Call the Activator to attempt instantiation the given object
		protected object createFromValue(object o, object[] args)
		{
			Type value = (o is Type) ? o as Type : o.GetType ();
			object retv = null;
			try
			{
				if (args == null || args.Length == 0)
				{
					retv = Activator.CreateInstance (value);
				}
				else
				{
					retv = Activator.CreateInstance(value, args);
				}
			}
			catch
			{
				//No-op
			}
			return retv;
		}
	}
}

