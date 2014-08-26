namespace strange.unittests
{
    public class ExtendedInheritanceOveride : BaseInheritanceOverride
    {
        [Inject]
        public new IExtendedInterface MyInterface { get; set; }
    }
}
