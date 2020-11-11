using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class EnumerableLocatorAdapter<T>
        : LocatorAdapterBase<T, IEnumerable<T>>
        where
            T : class
    {
        public EnumerableLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.Select((l) => (T)l.GetResourceInstance());
                }
                else
                {
                    return Enumerable.Empty<T>();
                }
            })
        { }
    }
}