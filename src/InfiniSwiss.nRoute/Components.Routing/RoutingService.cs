using nRoute.Internal;
using nRoute.SiteMaps;
using System;

namespace nRoute.Components.Routing
{
    public static class RoutingService
    {

        #region Declarations

        private const string INVALID_REQUEST_URL = "Request's url is invalid, routing url cannot be null or empty or start with a '/' or '~' character and it cannot contain a '?' character.";
        private const string NO_ROUTE_MATCHES = "The incoming request does not match any route.";
        private const string NO_ROUTE_HANDLER = "A RouteHandler must be specified for the selected route.";
        private const string INVALID_ROUTE_URL = "The request URL cannot start with a '/' or '~' character and it cannot contain a '?' character.";
        private const string SITEAREA_NOT_RESOLVED = "Could not resolve SiteMapArea '{0}' to resolve request for Url '{1}'";

        #endregion

        #region Handling Related

        public static void Resolve(IUrlRequest request, Action<IUrlResponse> responseCallback)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentNotNull(responseCallback, "responseCallback");

            OnResolveRequest(request, false, (c, s) =>
            {
                if (s != ResponseStatus.Success)
                {
                    var _response = new UrlResponse(request, s, null);
                    responseCallback(_response);
                }
                else
                {
                    // and we get the response, using a callback
                    OnResolveResponse(c, false, responseCallback);
                }
            });
        }

        #endregion

        #region Proccess Request and Response

        public static void ResolveContext(IUrlRequest request, Action<IRoutingContext> contextCallback)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentNotNull(contextCallback, "contextCallback");
            OnResolveRequest(request, true, (c, s) => contextCallback(c));
        }

        public static IDisposable ResolveResponse(IRoutingContext context, Action<IUrlResponse> responseCallback)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(responseCallback, "responseCallback");

            // and resolve
            return OnResolveResponse(context, true, responseCallback);
        }

        #endregion

        #region Validation

        //public static bool CanProcessRequest(IRoutingRequest request, bool validateUrl)
        //{
        //    
        //    Guard.ArgumentNotNull(request, "request");
        //    if (!ValidateUrl(request.RequestUrl, validateUrl)) return false;

        //    // we check if we get a route data
        //    return (RouteTable.Routes.GetRouteData(request) == null);

        //}

        public static bool CanProcessUrl(string url, bool throwException)
        {

            if (!ValidateUrl(url, throwException)) return false;

            // we check if we get a route data
            return (RouteTable.Routes.GetRouteData(new UrlRequest(url)) == null);

        }

        public static bool ValidateUrl(string url, bool throwException)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (throwException) Guard.ArgumentNotNullOrEmpty(url, "url");
                return false;
            }
            if ((url.StartsWith("~", StringComparison.Ordinal) || url.StartsWith("/", StringComparison.Ordinal)) ||
                (url.IndexOf('?') != -1))
            {
                if (throwException) Guard.ArgumentValue(true, "url", INVALID_ROUTE_URL);
                return false;
            }

            // all else
            return true;
        }

        #endregion

        #region Routes Related

        public static void MapRoute(RouteBase route)
        {
            MapRoute(null, route);
        }

        public static void MapRoute(string routeName, RouteBase route)
        {
            Guard.ArgumentNotNull(route, "route");

            RouteTable.Routes.Add(routeName, route);
        }

        public static void MapRoute(string url, IRouteHandler handler)
        {
            MapRoute(null, url, null, null, null, handler);
        }

        public static void MapRoute(string routeName, string url, IRouteHandler handler)
        {
            MapRoute(routeName, url, null, null, null, handler);
        }

        public static void MapRoute(string url, ParametersCollection defaults, IRouteHandler handler)
        {
            MapRoute(null, url, defaults, null, null, handler);
        }

        public static void MapRoute(string routeName, string url, ParametersCollection defaults, IRouteHandler handler)
        {
            MapRoute(routeName, url, defaults, null, null, handler);
        }

        public static void MapRoute(string url, ParametersCollection defaults, ParametersCollection constraints,
            IRouteHandler handler)
        {
            MapRoute(null, url, defaults, constraints, null, handler);
        }

        public static void MapRoute(string routeName, string url, ParametersCollection defaults, ParametersCollection constraints,
           IRouteHandler handler)
        {
            MapRoute(routeName, url, defaults, constraints, null, handler);
        }

        public static void MapRoute(string routeName, string url, ParametersCollection defaults,
            ParametersCollection constraints, ParametersCollection dataTokens, IRouteHandler handler)
        {
            // note_ name can be null or empty
            Guard.ArgumentNotNullOrEmpty(url, "url");
            Guard.ArgumentNotNull(handler, "handler");

            // we add
            MapRoute(routeName, new Route(url, defaults, constraints, dataTokens, handler));
        }

        #endregion

        #region Helpers

        private static void OnResolveRequest(IUrlRequest request, bool throwException, Action<IRoutingContext, ResponseStatus> requestCallback)
        {

            if (!string.IsNullOrEmpty(request.SiteArea) && !SiteMapService.IsSiteAreaInitialized(request.SiteArea))
            {
                SiteMapService.InitializeSiteArea(request.SiteArea, (a) =>
                {
                    if (a)
                    {
                        var _context = (IRoutingContext)null;
                        var _status = OnResolveRoute(request, throwException, out _context);
                        requestCallback(_context, _status);
                    }
                    else
                    {
                        if (throwException)
                        {
                            throw new SiteMapException(string.Format(SITEAREA_NOT_RESOLVED, request.SiteArea, request.RequestUrl),
                                request.SiteArea, request);
                        }
                        else
                        {
                            requestCallback(null, ResponseStatus.HandlerNotFound);
                        }
                    }
                });
            }
            else
            {
                var _context = (IRoutingContext)null;
                var _status = OnResolveRoute(request, throwException, out _context);
                requestCallback(_context, _status);
            }
        }

        private static ResponseStatus OnResolveRoute(IUrlRequest request, bool throwException, out IRoutingContext context)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentValue((!ValidateUrl(request.RequestUrl, false)), "request", INVALID_REQUEST_URL);

            // default value
            context = null;

            // we check the url
            if (!ValidateUrl(request.RequestUrl, throwException)) return ResponseStatus.UrlInvalid;

            // we get the route data first
            RouteData routeData = RouteTable.Routes.GetRouteData(request);
            if (routeData == null)
            {
                if (throwException) throw new InvalidOperationException(NO_ROUTE_MATCHES);
                return ResponseStatus.UrlNotFound;
            }

            // we get the route handler
            IRouteHandler routeHandler = routeData.RouteHandler;
            if (routeHandler == null)
            {
                if (throwException) throw new InvalidOperationException(NO_ROUTE_HANDLER);
                return ResponseStatus.HandlerNotFound;
            }

            // we merge the dictionaries (into the request parameters)
            var _requestParameters = new ParametersCollection();

            // we add the data-tokens specified with route, n_ote these can be overriden
            if (routeData.DataTokens != null && routeData.DataTokens.Count > 0)
            {
                // we add the values, from the routes data, if it exists then we update the value
                foreach (var _parameter in routeData.DataTokens)
                {
                    _requestParameters.Add(_parameter.Key, _parameter.Value);
                }
            }

            // we add the values found in the url merged with the default values, n_ote these can be overriden
            if (routeData.Values != null && routeData.Values.Count > 0)
            {
                // we add the values, from the routes data, if it exists then we update the value
                foreach (var _parameter in routeData.Values)
                {
                    if (_requestParameters.ContainsKey(_parameter.Key))
                    {
                        _requestParameters[_parameter.Key] = _parameter.Value;
                    }
                    else
                    {
                        _requestParameters.Add(_parameter.Key, _parameter.Value);
                    }
                }
            }

            // and our passed in parameters will override any existing entry
            if (request.RequestParameters != null && request.RequestParameters.Count > 0)
            {
                foreach (var _parameter in request.RequestParameters)
                {
                    if (_requestParameters.ContainsKey(_parameter.Key))
                    {
                        _requestParameters[_parameter.Key] = _parameter.Value;
                    }
                    else
                    {
                        _requestParameters.Add(_parameter.Key, _parameter.Value);
                    }
                }
            }

            // we setup the request cotext and get the response
            context = new RoutingContext(request, routeData, _requestParameters);
            return ResponseStatus.Success;
        }

        private static IDisposable OnResolveResponse(IRoutingContext context, bool throwException, Action<IUrlResponse> responseCallback)
        {
            Guard.ArgumentNotNull(responseCallback, "responseCallback");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentValue(context.Request == null, "context", "Context's request cannot be null");
            Guard.ArgumentValue(context.RouteData == null, "context", "Context's route data cannot be null.");

            // we try and get the response from the handler
            try
            {
                // we get the response from the route data, and we check if it is null or not
                var _observable = context.RouteData.RouteHandler.GetResponse(context);
                if (_observable == null)
                {
                    responseCallback(new UrlResponse(context.Request, ResponseStatus.HandlerNotFound, null));
                    return new RelayDisposable();
                }

                // and we subscribe to the 
                return _observable.Subscribe(responseCallback, (e) => new UrlResponse(context.Request, ResponseStatus.Exception, e));
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw ex;
                }
                else
                {
                    responseCallback(new UrlResponse(context.Request, ResponseStatus.Exception, ex));
                    return new RelayDisposable();
                }
            }
        }

        #endregion

    }
}
