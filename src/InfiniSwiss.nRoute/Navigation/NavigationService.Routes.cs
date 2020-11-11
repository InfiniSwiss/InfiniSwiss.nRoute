using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using nRoute.Navigation.Mapping;
using System;
using System.Collections.Specialized;

namespace nRoute.Navigation
{
    public static partial class NavigationService
    {
        private const string ABOUT_BLANK = "about:blank";
        private const string ABOUT_ERROR = "about:error";

        private static readonly Object _routesLock = new Object();

        #region Initialize Related

        private static void SetupRoutes()
        {
            // two built-in cases/handlers
            RoutingService.MapRoute(ABOUT_BLANK, new NavigationContentHandler((p) => new Themes.AboutBlank()));
            RoutingService.MapRoute(ABOUT_ERROR, new NavigationContentHandler((p) => new Themes.AboutError()));

            // this listens to any new elements being registered
            Resource<IRouteHandler>.Catalog.CollectionChanged += Route_CollectionChanged;
        }

        #endregion

        #region Manual Map Action Related

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
                    var _locator = e.NewItems[0] as IResourceLocator;
                    if (_locator == null) return;

                    // map the route
                    MapLocatorRoute(_locator);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    // we get the locator
                    var _locator = e.OldItems[0] as IResourceLocator;
                    if (_locator == null) return;

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
            var _navigationMeta = locator.ResourceMeta as NavigationMetaBase;
            if (_navigationMeta == null) return;

            // and we map it to the route, note_ we don't check for any pre-existing routes etc..
            MapRoute(locator.ResourceName, new Route(_navigationMeta.Url, (IRouteHandler)locator.GetResourceInstance()));
        }

        #endregion

    }
}
