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
 * @class strange.extensions.command.impl.CommandBinder
 * 
 * A Binder that triggers the instantiation of Commands.
 * 
 * Commands are where the logic of your application belongs.
 * These Commands typically focus on a single function, such as
 * adding a View, requesting a service, reading from or saving to a model.
 * 
 * The act of binding events to Commands means that code needn't know
 * anything about an event recipient, or even how the event will be used.
 * For example, a Mediator might send out an event that two View objects
 * collided. A Command would then determine that the result of that event
 * was to Destroy both objects, tell a ScoreKeeper model to change the
 * score and request a message be sent to the server. Whether that
 * example means one Command or three is up to your coding preference...
 * CommandBinder can trigger one Command or multiple Commands off the
 * same event.
 * 
 * Note that Strange also a features a Sequencer. CommandBinder fires all
 * Commands in parallel, while Sequencer runs them serially, with the
 * option of suspending the chain at any time.
 * 
 * Example bindings:

        Bind("someEvent").To<SomeCommand>(); //Works, but poor form to use strings. Use the next example instead

        Bind(EventMap.SOME_EVENT).To<SomeCommand>(); //Make it a constant

        Bind(ContextEvent.START).To<StartCommand>().Once(); //Destroy the binding immediately after a single use

 * 
 * See Command for details on asynchronous Commands.
 */

using System;
using System.Collections.Generic;
using strange.extensions.command.api;
using strange.extensions.injector.api;
using strange.framework.api;
using strange.extensions.injector.impl;
using strange.extensions.signal.impl;
using strange.extensions.signal.api;
using System.Reflection;
using UnityEngine;

namespace strange.extensions.command.impl
{
    public class SignalCommandBinder : CommandBinder
    {
        override protected void resolveBinding(IBinding binding, object key)
        {
            base.resolveBinding(binding, key);

            if (bindings.ContainsKey(key)) //If this key already exists, don't bind this again
            {
                IBaseSignal signal = (IBaseSignal)key;
                signal.AddListener(ReactTo); //Do normal bits, then assign the commandlistener to be reactTo
            }

        }

        protected override ICommand invokeCommand(Type cmd, ICommandBinding binding, object data, int depth)
        {
            IBaseSignal signal = (IBaseSignal)binding.key;
            ICommand command = createSignalCommand(cmd, data, signal.GetTypes()); //Special signal-only command creation
            command.sequenceId = depth;
            trackCommand(command, binding);
            executeCommand(command);
            return command;
        }

        protected ICommand createSignalCommand(object cmd, object data, List<Type> signalTypes)
        {

            injectionBinder.Bind<ICommand>().To(cmd);

            ICommand command = null;
            
            if (data != null)
            {

                object[] signalData = (object[])data;

                //Iterate each signal type, in order. 
                //Iterate values and find a match
                //If we cannot find a match, throw an error
                HashSet<Type> injectedTypes = new HashSet<Type>();
                List<object> values = new List<object>(signalData);

                foreach (Type type in signalTypes)
                {
                    if (!injectedTypes.Contains(type)) // Do not allow more than one injection of the same Type
                    {
                        bool foundValue = false;
                        foreach (object value in values)
                        {
                            if (value != null)
                            {

                                if (type.IsAssignableFrom(value.GetType())) //IsAssignableFrom lets us test interfaces as well
                                {

                                    injectionBinder.Bind(type).ToValue(value).ToInject(false);
                                    injectedTypes.Add(type);
                                    values.Remove(value);
                                    foundValue = true;
                                    break;
                                }
                            }
                            else //Do not allow null injections
                            {
                                throw new SignalException("Attempted to bind a null value to type: " + type, SignalExceptionType.COMMAND_NULL_INJECTION);
                            }
                        }
                        if (!foundValue)
                        {
                            throw new SignalException("Could not find an unused injectable value to inject in to signal command for Type: " + type, SignalExceptionType.COMMAND_VALUE_NOT_FOUND);
                        }
                    }
                    else
                    {
                        throw new SignalException("ActionCommandBinder: You have attempted to map more than one value of type: " + type +
                            " in ActionCommand: " + cmd.GetType() + ". Only the first value of a type will be injected. You may want to place your values in a VO, instead.",
                            SignalExceptionType.COMMAND_VALUE_CONFLICT);
                    }
                }
                command = injectionBinder.GetInstance<ICommand>() as ICommand;
                command.data = data; //Just to support swapping from EventCommand to ActionCommand more easily. No reason not to.
                
                foreach (Type typeToRemove in signalTypes) //clean up these bindings
                    injectionBinder.Unbind(typeToRemove);

            }
            else
            {
                command = injectionBinder.GetInstance<ICommand>() as ICommand;
                command.data = data; //Just to support swapping from EventCommand to ActionCommand more easily. No reason not to.
            }
            injectionBinder.Unbind<ICommand>();
            return command;
        }

        override public ICommandBinding Bind<T>()
        {
            IInjectionBinding binding = injectionBinder.GetBinding<T>();

            if (binding == null) //If this isn't injected yet, inject a new one as a singleton
            {
                injectionBinder.Bind<T>().ToSingleton();
            }

            T signal = (T)injectionBinder.GetInstance<T>();
            return base.Bind(signal);
        }


        /// <summary>Unbind by Signal Type</summary>
        /// <exception cref="InjectionException">If there is no binding for this type.</exception>
        public override void Unbind<T>()
        {
            ICommandBinding binding = (ICommandBinding) injectionBinder.GetBinding<T>();
            if (binding != null)
            {
                T signal = (T) injectionBinder.GetInstance<T>(); 
                Unbind(signal, null);
            }
        }

        /// <summary>
        /// Unbind by Signal Instance
        /// </summary>
        /// <param name="key"> Instance of IBaseSignal</param>
        override public void Unbind(object key, object name)
        {
            if (bindings.ContainsKey(key))
            {
                IBaseSignal signal = (IBaseSignal)key;
                signal.RemoveListener(ReactTo); 
            }
            base.Unbind(key, name);
        }
    }

}