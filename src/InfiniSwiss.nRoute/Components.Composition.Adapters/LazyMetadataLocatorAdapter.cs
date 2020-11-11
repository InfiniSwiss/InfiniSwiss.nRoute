namespace nRoute.Components.Composition.Adapters
{
    public class LazyMetadataLocatorAdapter<T, TMetadata>
        : LocatorAdapterBase<T, Lazy<T, TMetadata>>
        where
            T : class
        where
            TMetadata : class
    {
        public LazyMetadataLocatorAdapter()
            : base((s) =>
                       {
                           // note_ this will throw an exception if the resource with the given key is not found
                           // also note_ the use ResourceMeta as TMetadata
                           var _resourceLocator = string.IsNullOrEmpty(s)
                                                      ? ResourceLocator.GetResourceLocator<T>()
                                                      : ResourceLocator.GetResourceLocator<T>(s);
                           return new Lazy<T, TMetadata>(() => (T)_resourceLocator.GetResourceInstance(),
                                                          _resourceLocator.ResourceMeta as TMetadata);
                       })
        { }
    }
}
