namespace nRoute.ViewServices
{
    public enum ViewServiceLifetime
    {
        Singleton,
        //WeakSingleton,
        PerInstance,

        DiscoveredInstance,             // discovered from the visual tree
        SelfRegisteredInstance          // a view-service self registers itself
    }
}