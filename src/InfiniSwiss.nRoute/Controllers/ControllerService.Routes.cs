using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Controllers.Mapping;
using nRoute.Internal;
using System;
using System.Collections.Specialized;

namespace nRoute.Controllers
{
    public static partial class ControllerService
    {
        private static readonly object _routesLock = new();

        static ControllerService()
        {
            SetupRoutes();
        }

        #region Initialize Related

        private static void SetupRoutes()
        {
            // this listens to any new elements being registered
            Resource<IRouteHandler>.Catalog.CollectionChanged += Route_CollectionChanged;
        }

        internal static void Initialize()
        {
            MapAllRegisteredRoutes();
        }

        #endregion

        #region Manual Map Action Related

        public static void MapRoute(string url, Type controllerType)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            MapRoute(url, new ControllerHandler(controllerType));
        }

        public static void MapRoute(string url, Type controllerType, ParametersCollection defaults)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            MapRoute(url, defaults, new ControllerHandler(controllerType));
        }

        public static void MapRoute(string url, Type controllerType, ParametersCollection defaults, ParametersCollection constraints)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            MapRoute(url, defaults, constraints, new ControllerHandler(controllerType));
        }

        public static void MapRoute(RouteBase route)
        {
            MapRoute(null, route);
        }

        public static void MapRoute(string routeName, RouteBase route)
        {
            // note_ route name can be null or empty
            Guard.ArgumentNotNull(route, "route");

            // and we add 
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

        public static void MapRoute(string url, ParametersCollection defaults,
            ParametersCollection constraints, IRouteHandler handler)
        {
            MapRoute(null, url, defaults, constraints, null, handler);
        }

        public static void MapRoute(string routeName, string url, ParametersCollection defaults,
            ParametersCollection constraints, IRouteHandler handler)
        {
            MapRoute(routeName, url, defaults, constraints, null, handler);
        }

        public static void MapRoute(string routeName, string url, ParametersCollection defaults,
            ParametersCollection constraints, ParametersCollection dataTokens, IRouteHandler handler)
        {
            // note_ name can be null or empty
            Guard.ArgumentNotNullOrEmpty(url, "url");
            Guard.ArgumentNotNull(handler, "handler");

            // we add the route, and the handler is the action route handler
            var _route = new Route(url, defaults, constraints, dataTokens, handler);
            MapRoute(routeName, _route);
        }

        #endregion

        #region Catalog Related

        private static void Route_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (_routesLock)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    // we get the locator
                    if (e.NewItems[0] is not IResourceLocator _locator) return;

                    // map the route
                    MapLocatorRoute(_locator);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    // we get the locator
                    if (e.OldItems[0] is not IResourceLocator _locator) return;

                    // we get the route
                    var _route = RouteTable.Routes[_locator.ResourceName];
                    RouteTable.Routes.Remove(_route);
                }
            }
        }

        #endregion

        #region Helpers

        private static void MapAllRegisteredRoutes()
        {
            var _routeLocators = Resource<IRouteHandler>.Catalog;
            foreach (var _routeLocator in _routeLocators)
            {
                MapLocatorRoute(_routeLocator);
            }
        }

        private static void MapLocatorRoute(IResourceLocator locator)
        {
             if (locator.ResourceMeta is not ControllerMeta _navigationMeta) return;

            // and we map it to the route, note_ we don't check for any pre-existing routes etc..
            MapRoute(locator.ResourceName, new Route(_navigationMeta.Url, (IRouteHandler)locator.GetResourceInstance()));
        }

        #endregion

    }
}