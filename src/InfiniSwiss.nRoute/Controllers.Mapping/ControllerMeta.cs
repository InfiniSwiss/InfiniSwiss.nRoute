using nRoute.Internal;
using System;

namespace nRoute.Controllers.Mapping
{
    public class ControllerMeta
    {
        private readonly Type _controllerType;
        private readonly string _url;

        public ControllerMeta(Type controllerType, string url)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            Guard.ArgumentNotNullOrWhiteSpace(url, "url");

            _controllerType = controllerType;
            _url = url;
        }

        #region Properties

        public Type ControllerType
        {
            get { return _controllerType; }
        }

        public string Url
        {
            get { return _url; }
        }

        #endregion

    }
}
