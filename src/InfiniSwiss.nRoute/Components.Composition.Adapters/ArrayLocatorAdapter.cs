using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class ArrayLocatorAdapter<T>
        : LocatorAdapterBase<T, T[]>
        where
            T : class
    {
        public ArrayLocatorAdapter()
            : base((s) =>
            {
                // note_ we are ignoring the string "s" type sent in
                if (Resource.IsResourceRegistered(typeof(T), true))
                {
                    return Resource<T>.Catalog.Select((l) => (T)l.GetResourceInstance()).ToArray();
                }
                else
                {
                    return new T[] { };
                }
            })
        { }
    }
}