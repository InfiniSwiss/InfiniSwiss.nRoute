using nRoute.Internal;

namespace nRoute.Components.Routing
{
    public class RoutingContext
         : IRoutingContext
    {
        private readonly IUrlRequest _request;
        private readonly RouteData _routeData;
        private readonly ParametersCollection _parsedParameters;

        public RoutingContext(IUrlRequest request, RouteData routeData, ParametersCollection parsedParameters)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentNotNull(routeData, "routeData");
            Guard.ArgumentNotNull(parsedParameters, "parsedParameters");

            _request = request;
            _routeData = routeData;
            _parsedParameters = parsedParameters;

        }

        #region IRoutingContext Members

        public IUrlRequest Request
        {
            get { return _request; }
        }

        public RouteData RouteData
        {
            get { return _routeData; }
        }

        public ParametersCollection ParsedParameters
        {
            get { return _parsedParameters; }
        }

        #endregion

    }
}
