using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class QueryableLocatorAdapter<T>
        : LocatorAdapterBase<T, IQueryable<T>>
        where
            T : class
    {
        public QueryableLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.AsQueryable().Select((l) => (T)l.GetResourceInstance());
                }
                else
                {
                    return Enumerable.Empty<T>().AsQueryable();
                }
            })
        { }
    }
}