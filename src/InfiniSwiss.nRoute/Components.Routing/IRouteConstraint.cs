namespace nRoute.Components.Routing
{
    public interface IRouteConstraint
    {

        bool Match(IUrlRequest request, Route route, string parameterName,
            ParametersCollection values, RouteDirection routeDirection);
    }

}
