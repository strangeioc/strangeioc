using System;
using System.Collections.Generic;
using strange.extensions.sequencer.api;
using strange.extensions.command.api;
using strange.framework.api;
using strange.framework.impl;
using strange.extensions.injector.api;
using strange.extensions.signal.api;
using strange.extensions.signal.impl;

namespace strange.extensions.sequencer.impl
{
	class SignalSequencer: Sequencer
	{
		public SignalSequencer ()
		{
		}

        override protected void resolveBinding(IBinding binding, object key)
        {
            base.resolveBinding(binding, key);

            if (bindings.ContainsKey(key)) //If this key already exists, don't bind this again
            {
                IBaseSignal signal = (IBaseSignal)key;
                signal.AddListener(ReactTo); //Do normal bits, then assign the commandlistener to be reactTo
            }
        }

        override protected ISequenceCommand createCommand(object cmd, object data)
        {
            injectionBinder.Bind<ICommand>().To(cmd);

            ISequenceCommand command = null;
            if (data != null)
            {

                object[] values = (object[])data;

                //TODO: If we find two objects of the same Type, we should throw a debugwarning to announce that one of them will not be used.
                HashSet<Type> types = new HashSet<Type>();
                int len = values.Length;
                for (int i = 0; i < len; i++)
                {
                    object value = values[i];
                    Type valueType = value.GetType();
                    if (!types.Contains(valueType))
                    {
                        injectionBinder.Bind(valueType).ToValue(value).ToInject(false); //Do not attempt to inject IN TO these values. They will still be injected properly in to other injected objects.
                        types.Add(valueType);
                    }
                    else
                    {
                        throw new SignalException("ActionCommandBinder: You have attempted to map more than one value of type: " + valueType +
                            " in ActionCommand: " + command.GetType() + ". Only the first value of a type will be injected. You may want to place your values in a VO, instead.",
                            SignalExceptionType.COMMAND_VALUE_CONFLICT);
                    }

                }

                command = injectionBinder.GetInstance<ISequenceCommand>() as ISequenceCommand;
                command.data = data; //Just to support swapping from EventCommand to ActionCommand more easily. No reason not to.

                for (int i = 0; i < len; i++)
                {
                    object value = values[i];
                    injectionBinder.Unbind(value.GetType());
                }

            }
            else
            {
                command = injectionBinder.GetInstance<ISequenceCommand>() as ISequenceCommand;
                command.data = data; //Just to support swapping from EventCommand to ActionCommand more easily. No reason not to.
            }
            injectionBinder.Unbind<ICommand>();
            return command;
        }

        override public ISequenceBinding Bind<T>()
        {
            IInjectionBinding binding = injectionBinder.GetBinding<T>();

            if (binding == null) //If this isn't injected yet, inject a new one as a singleton
                injectionBinder.Bind<T>().ToSingleton();

            T signal = (T)injectionBinder.GetInstance<T>();
            return base.Bind(signal) as ISequenceBinding;
        }


        /// <summary>Unbind by Signal Type</summary>
        /// <exception cref="InjectionException">If there is no binding for this type.</exception>
        public override void Unbind<T>()
        {
            ICommandBinding binding = (ICommandBinding)injectionBinder.GetBinding<T>();
            if (binding != null)
            {
                T signal = (T)injectionBinder.GetInstance<T>();
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
