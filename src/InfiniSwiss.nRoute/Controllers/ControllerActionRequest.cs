using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Navigation;
using System;

namespace nRoute.Controllers
{
    public class ControllerActionRequest : UrlRequest
    {
        private readonly object _sender;
        private readonly INavigationHandler _handler;

        #region Constructors

        public ControllerActionRequest(String requestUrl)
            : this(requestUrl, null, null, null, null) { }

        public ControllerActionRequest(String requestUrl, string siteArea)
            : this(requestUrl, null, siteArea, null, null) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters)
            : this(requestUrl, requestParameters, null, null, null) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters, string siteArea)
            : this(requestUrl, requestParameters, siteArea, null, null) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters, INavigationHandler handler)
            : this(requestUrl, requestParameters, null, handler, null) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters, string siteArea, INavigationHandler handler)
            : this(requestUrl, requestParameters, siteArea, handler, null) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters, Object sender)
            : this(requestUrl, requestParameters, null, null, sender) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters, string siteArea, Object sender)
            : this(requestUrl, requestParameters, siteArea, null, sender) { }

        public ControllerActionRequest(String requestUrl, ParametersCollection requestParameters, string siteArea, INavigationHandler handler, Object sender)
            : base(requestUrl, requestParameters, siteArea)
        {
            _sender = sender;
            _handler = handler;
        }

        #endregion

        #region Properties

        public object Sender
        {
            get { return _sender; }
        }

        public INavigationHandler Handler
        {
            get { return _handler; }
        }

        #endregion

    }
}
