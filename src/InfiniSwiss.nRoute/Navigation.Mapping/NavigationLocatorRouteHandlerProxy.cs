using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Navigation.Mapping
{
    /// <remarks>
    /// Since the route handler is stored with the RLF, we use this proxy to register with the routing engine
    /// and it fetches the response from the RLF, if not found then it returns a handler not found response
    /// </remarks>
    public class NavigationLocatorRouteHandlerProxy
         : IRouteHandler
    {
        private readonly string _url;

        public NavigationLocatorRouteHandlerProxy(string url)
        {
            Guard.ArgumentNotNullOrWhiteSpace(url, "url");
            _url = url;
        }

        #region IRouteHandler Members

        public IObservable<IUrlResponse> GetResponse(IRoutingContext context)
        {
            // we get the route handler from the resource locator
            IRouteHandler _routeHandler = null;
            if (!ResourceLocator.TryGetResource<IRouteHandler>(_url, out _routeHandler))
            {
                // we return that the handler was not found
                var _observer = new LazyRelayObservable<IUrlResponse>((s) =>
                {
                    s.OnNext(new UrlResponse(context.Request, ResponseStatus.HandlerNotFound, null, null));
                    s.OnCompleted();
                });
                return _observer;
            }

            // we we resolve
            return _routeHandler.GetResponse(context);
        }

        #endregion

    }
}