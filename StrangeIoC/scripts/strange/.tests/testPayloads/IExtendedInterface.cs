namespace strange.unittests
{
    public interface IExtendedInterface : ISimpleInterface
    {
        bool Extended();
    }

    public interface IExtendedInterfaceTwo : IExtendedInterface
    {
        bool ExtendedTwo();
    }
}
