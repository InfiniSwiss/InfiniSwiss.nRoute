using System;

namespace nRoute.Components.Composition
{
    public interface IResourceLocator
        : IDisposable
    {
        string ResourceName { get; }

        Object ResourceMeta { get; }

        Object GetResourceInstance();
    }
}
