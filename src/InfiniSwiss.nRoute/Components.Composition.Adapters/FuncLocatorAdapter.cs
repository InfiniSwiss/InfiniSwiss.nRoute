using System;

namespace nRoute.Components.Composition.Adapters
{
    public class FuncLocatorAdapter<T>
        : LocatorAdapterBase<T, Func<T>>
        where
            T : class
    {
        public FuncLocatorAdapter()
            : base((s) =>
            {
                // note_ this will throw an exception if the resource with the given key is not found
                // also here we have cached the ResourceLocator instance, this means no additional round trips
                var _resourceLocator = string.IsNullOrEmpty(s)
                                           ? ResourceLocator.GetResourceLocator<T>()
                                           : ResourceLocator.GetResourceLocator<T>(s);
                return new Func<T>(() => (T)_resourceLocator.GetResourceInstance());
            })
        { }
    }
}
