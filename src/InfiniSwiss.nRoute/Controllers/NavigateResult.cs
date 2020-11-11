using nRoute.Components;
using nRoute.Internal;
using nRoute.Navigation;

namespace nRoute.Controllers
{
    public class NavigateResult
        : ActionResult
    {
        private readonly string _url;
        private readonly string _siteArea;
        private readonly ParametersCollection _requestParameters;
        private readonly INavigationHandler _handler;

        public NavigateResult(string url)
            : this(url, null, null, null) { }

        public NavigateResult(string url, string siteArea)
            : this(url, null, siteArea, null) { }

        public NavigateResult(string url, ParametersCollection requestParameters)
            : this(url, requestParameters, null, null) { }

        public NavigateResult(string url, ParametersCollection requestParameters, string siteArea)
            : this(url, requestParameters, siteArea, null) { }

        public NavigateResult(string url, ParametersCollection requestParameters, INavigationHandler handler)
            : this(url, requestParameters, null, handler) { }

        public NavigateResult(string url, ParametersCollection requestParameters, string siteArea, INavigationHandler handler)
        {
            Guard.ArgumentNotNullOrWhiteSpace(url, "url");

            _url = url;
            _siteArea = siteArea;
            _requestParameters = requestParameters;
            _handler = handler;
        }

        #region Overrides

        public override void ExecuteResult(ControllerContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            var _request = new NavigationRequest(_url, _requestParameters, _siteArea, NavigateMode.New);
            if (_handler != null)
            {
                NavigationService.Navigate(_request, _handler);
            }
            else if (context.Request.Handler != null)
            {
                NavigationService.Navigate(_request, context.Request.Handler);
            }
            else
            {
                NavigationService.Navigate(_request);
            }
        }

        #endregion

    }
}
