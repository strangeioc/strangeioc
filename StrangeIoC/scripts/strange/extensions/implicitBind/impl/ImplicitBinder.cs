
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using strange.extensions.implicitBind.api;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using Twitch;

namespace strange.extensions.implicitBind.impl
{
    public class ImplicitBinder : IImplicitBinder
    {

        [Inject]
        public IInjectionBinder injectionBinder { get; set; }

        [Inject]
        public IMediationBinder mediationBinder { get; set; }


        //Hold a copy of the assembly so we aren't retrieving this multiple times. 
        private Assembly assembly;


        [PostConstruct]
        public void PostConstruct()
        {
            assembly = Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Search through indicated namespaces and scan for all annotated classes.
        /// Automatically create bindings
        /// </summary>
        /// <param name="usingNamespaces">Array of namespaces. Compared using StartsWith. </param>

        public virtual void ScanForAnnotatedClasses(string[] usingNamespaces)
        {
            if (assembly != null)
            {

                IEnumerable<Type> types = assembly.GetExportedTypes();

                List<Type> typesInNamespaces = new List<Type>();
                int namespacesLength = usingNamespaces.Length;
                for (int ns = 0; ns < namespacesLength; ns++)
                {
                    typesInNamespaces.AddRange(types.Where(t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(usingNamespaces[ns])));
                }

                List<Tuple3<Type,Type, bool> > implementsBindings = new List<Tuple3<Type, Type, bool>>();
                List<Tuple3<Type, Type, bool>> implementedByBindings = new List<Tuple3<Type, Type, bool>>();

                foreach (Type type in typesInNamespaces)
                {
                    object[] implements = type.GetCustomAttributes(typeof (Implements), true);
                    object[] implementedBy = type.GetCustomAttributes(typeof(ImplementedBy), true);
                    object[] mediated = type.GetCustomAttributes(typeof(MediatedBy), true);
                    object[] mediates = type.GetCustomAttributes(typeof(Mediates), true);

                    #region Concrete and Interface Bindings

                    Type bindType = null;
                    Type toType = null;
                    //Interfaces first
                    if (implementedBy.Any())
                    {

                        ImplementedBy implBy = (ImplementedBy)implementedBy.First();
                        if (implBy.DefaultType.GetInterfaces().Contains(type)) //Verify this DefaultType exists and implements the tagged interface
                        {
                            implementedByBindings.Add(new Tuple3<Type, Type, bool>(type, implBy.DefaultType, implBy.Scope == InjectionBindingScope.CROSS_CONTEXT));
                        }
                        else
                        {
                            throw new InjectionException("Default Type: " + implBy.DefaultType.Name + " does not implement annotated interface " + type.Name,
                                InjectionExceptionType.IMPLICIT_BINDING_DEFAULT_TYPE_DOES_NOT_IMPLEMENT_INTERFACE);
                        }

                    }

                    if (implements.Any())
                    {
                        Implements impl = (Implements)implements.First();
                        Type[] interfaces = type.GetInterfaces();

                        //Confirm this type implements the type specified
                        if (impl.DefaultInterface != null)
                        {
                            if (interfaces.Contains(impl.DefaultInterface)) //Verify this Type implements the passed interface
                            {
                                implementsBindings.Add(new Tuple3<Type, Type, bool>(impl.DefaultInterface, type, impl.Scope == InjectionBindingScope.CROSS_CONTEXT));
                            }
                            else
                            {
                                throw new InjectionException("Annotated type " + type.Name + " does not implement Default Interface " + impl.DefaultInterface.Name,
                                    InjectionExceptionType.IMPLICIT_BINDING_TYPE_DOES_NOT_IMPLEMENT_DEFAULT_INTERFACE);
                            }
                        }
                        else //Concrete
                        {
                            implementsBindings.Add(new Tuple3<Type, Type, bool>(type, null, impl.Scope == InjectionBindingScope.CROSS_CONTEXT));
                        }
                    }

                    #endregion
                    
                    //Handle mediations here. We have no need to re-iterate over them to prioritize anything. Not yet, at least.

                    #region Mediations

                    Type mediatorType = null;
                    Type viewType = null;
                    if (mediated.Any())
                    {
                        viewType = type;
                        mediatorType = ((MediatedBy)mediated.First()).MediatorType;

                        if (mediatorType == null)
                            throw new MediationException("Cannot implicitly bind view of type: " + type.Name + " due to null MediatorType",
                                MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);
                    }
                    else if (mediates.Any())
                    {
                        mediatorType = type;
                        viewType = ((Mediates)mediates.First()).ViewType;

                        if (viewType == null)
                            throw new MediationException("Cannot implicitly bind Mediator of type: " + type.Name + " due to null ViewType",
                                MediationExceptionType.MEDIATOR_VIEW_STACK_OVERFLOW);
                    }

                    if (mediationBinder != null && viewType != null && mediatorType != null) //Bind this mediator!
                        mediationBinder.Bind(viewType).To(mediatorType);

                    #endregion
                }

                //implementedBy/interfaces first, then implements to give them priority (they will overwrite)
                implementedByBindings.ForEach(Bind);
                //Next implements tags, which have priority over interfaces
                implementsBindings.ForEach(Bind);
            }
            else
            {
                throw new InjectionException("Assembly was not initialized yet for Implicit Bindings!", InjectionExceptionType.UNINITIALIZED_ASSEMBLY);
            }
        }

        private void Bind(Tuple3<Type, Type, bool> toBind)
        {
            //We do not check for the existence of a binding. Because implicit bindings are weak bindings, they are overridden automatically by other implicit bindings
            //Therefore, ImplementedBy will be overriden by an Implements to that interface.
            IInjectionBinding binding = toBind.Second != null ?
                injectionBinder.Bind(toBind.First).To(toBind.Second).ToSingleton() :
                injectionBinder.Bind(toBind.First).ToSingleton();
            if (toBind.Third) //Bind this to the cross context injector
            {
                binding.CrossContext();
            }
            binding.Weak();
        }
    }
}
