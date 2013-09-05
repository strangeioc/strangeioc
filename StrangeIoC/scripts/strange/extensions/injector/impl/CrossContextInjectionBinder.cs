using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.framework.api;

public class CrossContextInjectionBinder : InjectionBinder, ICrossContextInjectionBinder
{
    /// Cross Context Injector is shared with all child contexts.
    public IInjectionBinder CrossContextBinder { get; set; }

    public CrossContextInjectionBinder() : base()
    {
    }
    public override IInjectionBinding GetBinding<T>()
    {
        return GetBinding(typeof(T), null);
    }

    public override IInjectionBinding GetBinding(object key, object name)
    {
        
        IInjectionBinding binding = base.GetBinding(key, name) as IInjectionBinding;


        if (binding == null) //Attempt to get this from the cross context. Cross context is always SECOND PRIORITY. Local injections always override
        {
            if (CrossContextBinder != null)
            {
                binding = CrossContextBinder.GetBinding(key, name) as IInjectionBinding;
            }
        }

        return binding;
    }

    override public void resolveBinding(IBinding binding, object key)
    {
        //Decide whether to resolve locally or not
        if (binding is IInjectionBinding)
        {
            InjectionBinding injectionBinding = (InjectionBinding)binding;
            if (injectionBinding.isCrossContext)
            {

                if (CrossContextBinder == null) //We are a crosscontextbinder
                {
                    base.resolveBinding(binding, key);
                }
                else 
                {
                    Unbind(key); //remove this cross context binding from the local binder
                    CrossContextBinder.resolveBinding(binding, key);
                }
            }
            else
            {
                base.resolveBinding(binding, key);
            }
        }
       

    }
}