using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class EnumerableLazyMetaLocatorAdapter<T, TMetadata>
        : LocatorAdapterBase<T, IEnumerable<Lazy<T, TMetadata>>>
        where
            T : class
        where
            TMetadata : class
    {
        public EnumerableLazyMetaLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in - and use of as TMetadata
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.Select((l) =>
                    {
                        return new Lazy<T, TMetadata>(
                            () => (T)l.GetResourceInstance(), l.ResourceMeta as TMetadata);
                    }).Where((r) => r.Metadata != null);
                }
                else
                {
                    return Enumerable.Empty<Lazy<T, TMetadata>>();
                }
            })
        { }
    }
}