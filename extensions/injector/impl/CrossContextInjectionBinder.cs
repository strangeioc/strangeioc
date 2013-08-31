using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.framework.api;
using System;

public class CrossContextInjectionBinder : InjectionBinder, ICrossContextInjectionBinder
{
    /// Cross Context Injector is shared with all child contexts.
    public IInjectionBinder CrossContextBinder { get; set; }

    public static int WTF = 0;

    public int MYWTF = -1;

    public CrossContextInjectionBinder() : base()
    {
        WTF++;
        MYWTF = WTF;
        System.Console.Write("Instantiating a cross context injectionbinder\n");


    }
    public override IInjectionBinding GetBinding<T>()
    {
        return GetBinding(typeof(T), null);
    }

    public override IInjectionBinding GetBinding(object key, object name)
    {
        
        IInjectionBinding binding = base.GetBinding(key, name) as IInjectionBinding;

        if (binding != null && CrossContextBinder == null)
        {
            System.Console.Write("I AM CROSS CONTEXTOMG OMG OGMI am cross context with MYWTF: " + MYWTF + " and I found a binding: " + binding + " \n");
        }

        if (CrossContextBinder != null)
            binding = null;
        if (CrossContextBinder != null)
        {

            if (binding == null) //Attempt to get this from the cross context. Cross context is always SECOND PRIORITY. Local injections always override
            {
                if (CrossContextBinder != null)
                {
                    System.Console.Write("binding is null and cross context binder is not null and my WTF is: " + MYWTF + " \n");
                    binding = CrossContextBinder.GetBinding(key, name) as IInjectionBinding;
                }
                else
                {
                    throw new InjectionException("Cross Context Injector is null while attempting to resolve a cross context binding", InjectionExceptionType.MISSING_CROSS_CONTEXT_INJECTOR);
                }
            }
        }

        return binding;
    }

    override protected void resolveBinding(IBinding binding, object key)
    {
        System.Console.Write("resolve binding on binding: " + binding + "with key: " + key + " \n");
        //Decide whether to resolve locally or not
        if (binding is IInjectionBinding)
        {
            InjectionBinding injectionBinding = (InjectionBinding)binding;
            if (injectionBinding.isCrossContext)
            {
                if (CrossContextBinder != null)
                {
                    //System.Console.Write("unbinding previous thingy \n");
                    //Unbind(binding); //Remove it locally if it exists

                    IInjectionBinding crossBinding = null;
                    if (injectionBinding.type.Equals(InjectionBindingType.VALUE))
                    {
                        crossBinding = ((IInjectionBinding)CrossContextBinder.Bind(key)).ToValue(binding.value); //.Named(binding.name);
                    }
                    else if (injectionBinding.type.Equals(InjectionBindingType.SINGLETON))
                    {
                        System.Console.Write("AM I EVER GOING IN TO HERE WTF? \n");
                        IInjectionBinding previousBinding = CrossContextBinder.GetBinding((Type)key);
                        if (previousBinding != null)
                        {
                            System.Console.Write("previous binding exists \n");
                            return;
                        }
                        else
                        {
                            crossBinding = binding as InjectionBinding;
                            crossBinding = injectionBinding;
                            object o = CrossContextBinder.injector.Instantiate(crossBinding);
                            System.Console.Write("previous binding does not exist \n");
                            //crossBinding = ((IInjectionBinding)CrossContextBinder.Bind((Type)key)).ToSingleton();
                        }
                        //object instance = CrossContextBinder.GetInstance((Type)key);
                        //System.Console.Write("binding singleton in resoolve binding, the key is: " + key + " and instance is: " + instance + " and instance type is: " + instance.GetType() +"\n");
                        //crossBinding = ((IInjectionBinding)CrossContextBinder.Bind(key)).ToValue( instance);
                    }
                    else
                    {
                        crossBinding = ((IInjectionBinding)CrossContextBinder.Bind(key)).To(binding.value);
                    }
                    crossBinding.ToInject(injectionBinding.toInject);
                }
                else
                {
                    throw new InjectionException("Cross Context Injector is null while attempting to resolve a cross context binding", InjectionExceptionType.MISSING_CROSS_CONTEXT_INJECTOR);
                }
            }
            else
            {
                System.Console.Write("CCIB about to resolve due to not being cross context \n");
                base.resolveBinding(binding, key);
            }
        }
       

    }
}