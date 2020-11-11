using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Controllers.Mapping
{
    public class DefaultControllerLocator
        : ResourceLocatorBase<IRouteHandler, ControllerMeta>
    {
        private readonly Object _lock = new Object();

        public DefaultControllerLocator(ControllerMeta meta)
            : base(meta.Url, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");
        }

        #region Override

        public override IRouteHandler GetResourceInstance()
        {
            return CreateControllerHandlerInstance();
        }

        #endregion

        #region Helpers

        protected virtual IRouteHandler CreateControllerHandlerInstance()
        {
            return new ControllerHandler(ResourceMeta.ControllerType);
        }

        #endregion

    }
}
