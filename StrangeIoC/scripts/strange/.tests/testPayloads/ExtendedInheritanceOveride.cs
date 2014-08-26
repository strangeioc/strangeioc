namespace strange.unittests
{
    public class ExtendedInheritanceOveride : BaseInheritanceOverride
    {
        [Inject]
        public new IExtendedInterface MyInterface { get; set; }
    }

    public class ExtendedInheritanceImplied : BaseInheritanceOverride
    {
        [Inject]
        public IExtendedInterface MyInterface { get; set; }
    }

}
