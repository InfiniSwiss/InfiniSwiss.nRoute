using System;

namespace nRoute.Components.Composition.Adapters
{
    public class LazyLocatorAdapter<T>
        : LocatorAdapterBase<T, Lazy<T>>
        where
            T : class
    {
        public LazyLocatorAdapter()
            : base((s) =>
            {
                // note_ this will throw an exception if the resource with the given key is not found
                // also here we have cached the ResourceLocator instance, this means no additional round trips
                var _resourceLocator = string.IsNullOrEmpty(s)
                                           ? ResourceLocator.GetResourceLocator<T>()
                                           : ResourceLocator.GetResourceLocator<T>(s);

                // note_ this will throw an exception if the resource with the given key is not found
                return new Lazy<T>(() => (T)(_resourceLocator.GetResourceInstance()));
            })
        { }
    }
}
