using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Controllers.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MapControllerAttribute
        : MapResourceBaseAttribute
    {
        private const string CONTROLLER_MUSTBE_OFTYPE = "{0} type must be an implementation of IController type.";

        private readonly string _url;

        public MapControllerAttribute(string url)
        {
            Guard.ArgumentNotNullOrWhiteSpace(url, "url");
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
            return typeof(IRouteHandler);
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            Guard.ArgumentValue(!typeof(IController).IsAssignableFrom(targetType), "targetType", CONTROLLER_MUSTBE_OFTYPE, targetType.FullName);

            return new DefaultControllerLocator(new ControllerMeta(targetType, _url));
        }

        #endregion

    }
}
