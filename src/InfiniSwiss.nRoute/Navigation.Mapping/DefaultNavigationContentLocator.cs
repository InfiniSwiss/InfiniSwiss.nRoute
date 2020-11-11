using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Navigation.Mapping
{
    public class DefaultNavigationContentLocator
         : DefaultNavigationLocatorBase
    {
        private readonly NavigationContentMeta _meta;

        public DefaultNavigationContentLocator(NavigationContentMeta meta)
            : base(meta)
        {
            Guard.ArgumentNotNull(meta, "meta");
            _meta = meta;
        }

        #region Overrides

        protected override IRouteHandler CreateRouteHandler()
        {
            return new NavigationContentHandler(CreateNavigationTypeInstance);
        }

        protected virtual Object CreateNavigationTypeInstance(ParametersCollection parameters)
        {
            return TypeBuilder.BuildType(_meta.ImplementationType);
        }

        #endregion

    }
}