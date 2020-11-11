using nRoute.Components.Routing;
using nRoute.Internal;

namespace nRoute.Navigation.Mapping
{
    public class DefaultNavigationResourceLocator
         : DefaultNavigationLocatorBase
    {
        private readonly NavigationResourceMeta _meta;

        public DefaultNavigationResourceLocator(NavigationResourceMeta navigationResourMeta)
         : base(navigationResourMeta)
        {
            Guard.ArgumentNotNull(navigationResourMeta, "navigationResourMeta");
            _meta = navigationResourMeta;
        }

        #region Overrides

        protected override IRouteHandler CreateRouteHandler()
        {
            return new NavigationResourceHandler(_meta.ResourcePath, _meta.ResourceAssembyName, _meta.ResourceLoader);
        }

        #endregion

    }
}