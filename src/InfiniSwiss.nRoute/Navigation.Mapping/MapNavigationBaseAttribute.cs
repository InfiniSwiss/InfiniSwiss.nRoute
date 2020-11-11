using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Navigation.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public abstract class MapNavigationBaseAttribute
         : MapResourceBaseAttribute
    {
        private readonly string _url;

        protected MapNavigationBaseAttribute(string url)
        {
            Guard.ArgumentNotNullOrEmpty(url, "url");
            _url = url;
        }

        #region Properties

        public string Url
        {
            get { return _url; }
        }

        #endregion

        #region Overrides

        protected internal override Type GetResourceType(Type targetType)
        {
            // so in RLF, this is registered with IRouteHandler resource
            return typeof(IRouteHandler);
        }

        #endregion

    }
}