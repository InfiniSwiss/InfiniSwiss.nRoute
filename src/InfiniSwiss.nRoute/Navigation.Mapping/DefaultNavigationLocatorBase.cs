using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;

namespace nRoute.Navigation.Mapping
{
    public abstract class DefaultNavigationLocatorBase
         : ResourceLocatorBase<IRouteHandler, NavigationMetaBase>
    {
        private IRouteHandler _routeHandler;

        protected DefaultNavigationLocatorBase(NavigationMetaBase meta)
            : base(meta.Url, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");
        }

        #region Overrides

        public override IRouteHandler GetResourceInstance()
        {
            // note_ we are caching the route handler 
            if (_routeHandler == null) _routeHandler = CreateRouteHandler();
            return _routeHandler;
        }

        #endregion

        #region Must Override

        protected abstract IRouteHandler CreateRouteHandler();

        #endregion

    }
}