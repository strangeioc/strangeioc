namespace strange.unittests
{
    public class ExtendedInheritanceOverride : BaseInheritanceOverride
    {
        [Inject]
        public new IExtendedInterface MyInterface { get; set; }
    }

    public class ExtendedInheritanceOverrideTwo : ExtendedInheritanceOverride
    {
        [Inject]
        public new IExtendedInterfaceTwo MyInterface { get; set; }
    }
    
    public class ExtendedInheritanceImplied : BaseInheritanceOverride
    {
        [Inject]
        public IExtendedInterface MyInterface { get; set; }
    }

    public class ExtendedInheritanceNoHide : BaseInheritanceOverride
    {
        
    }





}
