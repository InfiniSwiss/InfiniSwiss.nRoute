using nRoute.Components.Composition;

namespace nRoute.Modules
{
    [MapAsKnownResource]
    public interface IModule
    {
        void Initialize();
    }
}
