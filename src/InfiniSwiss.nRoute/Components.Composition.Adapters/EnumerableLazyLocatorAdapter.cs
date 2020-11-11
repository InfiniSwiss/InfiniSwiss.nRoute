using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class EnumerableLazyLocatorAdapter<T>
        : LocatorAdapterBase<T, IEnumerable<Lazy<T>>>
        where
            T : class
    {
        public EnumerableLazyLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.Select((l) => new Lazy<T>(() => (T)l.GetResourceInstance()));
                }
                else
                {
                    return Enumerable.Empty<Lazy<T>>();
                }
            })
        { }
    }
}