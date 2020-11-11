using System;

namespace nRoute.Components.Composition
{
    public interface IResourceResolver
    {
        Object Resolve(Type targetType);
    }
}
