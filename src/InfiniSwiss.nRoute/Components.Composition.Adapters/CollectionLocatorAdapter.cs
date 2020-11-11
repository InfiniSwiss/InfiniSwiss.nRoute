using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class CollectionLocatorAdapter<T>
        : LocatorAdapterBase<T, ICollection<T>>
        where
            T : class
    {
        public CollectionLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.Select((l) => (T)l.GetResourceInstance()).ToList();
                }
                else
                {
                    return Enumerable.Empty<T>().ToList();
                }
            })
        { }
    }
}