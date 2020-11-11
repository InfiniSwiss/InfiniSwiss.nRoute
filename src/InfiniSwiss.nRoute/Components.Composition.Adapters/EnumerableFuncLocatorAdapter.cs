using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class EnumerableFuncLocatorAdapter<T>
        : LocatorAdapterBase<T, IEnumerable<Func<T>>>
        where
            T : class
    {
        public EnumerableFuncLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.Select((l) => new Func<T>(() => (T)l.GetResourceInstance()));
                }
                else
                {
                    return Enumerable.Empty<Func<T>>();
                }
            })
        { }
    }
}