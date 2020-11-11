namespace nRoute.Components.Composition.Adapters
{
    public class FutureLocatorAdapter<T>
        : LocatorAdapterBase<T, Future<T>>
        where
            T : class
    {
        public FutureLocatorAdapter()
            : base((s) =>
            {
                // note_ this will throw an exception if the resource with the given key is not found
                return new Future<T>(
                    () => string.IsNullOrEmpty(s)
                        ? Resource.IsResourceRegistered(typeof(T), true)
                        : Resource.IsResourceRegistered(typeof(T), s),
                    () => (T)(string.IsNullOrEmpty(s)
                        ? ResourceLocator.GetResourceLocator<T>()
                        : ResourceLocator.GetResourceLocator<T>(s)).GetResourceInstance()
                );
            }
        )
        { }
    }
}
