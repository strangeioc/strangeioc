using strange.extensions.injector.api;

namespace strange.extensions.listBind.api
{
    public interface IListItemBinding
    {
        object Resolve(IInjectionBinder injectionBinder);

        void ToSingleton();
    }
}
