namespace nRoute.Components.Routing
{
    public interface IRoutingContext
    {
        IUrlRequest Request { get; }

        RouteData RouteData { get; }

        ParametersCollection ParsedParameters { get; }
    }
}
