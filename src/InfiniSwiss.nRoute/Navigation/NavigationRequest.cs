using nRoute.Components;
using nRoute.Components.Routing;

namespace nRoute.Navigation
{
    public class NavigationRequest
         : UrlRequest
    {
        //private const string BACKFORWARDREFRESH_SUPPORTED = "Only back, forward, or refresh navigation modes are supported";

        private NavigateMode? _navigateMode;

        #region Constructors

        //public NavigationRequest(NavigateMode navigationMode)
        //{
        //    Guard.ArgumentValue(navigationMode != NavigateMode.Back || navigationMode != NavigateMode.Forward || navigationMode != NavigateMode.Refresh, 
        //        "navigationMode", BACKFORWARDREFRESH_SUPPORTED);
        //    _navigateMode = navigationMode;
        //}

        public NavigationRequest(string url)
            : this(url, null, null) { }

        public NavigationRequest(string url, string siteArea)
            : this(url, null, siteArea) { }

        public NavigationRequest(string url, ParametersCollection requestParameters)
            : this(url, requestParameters, null) { }

        public NavigationRequest(string url, ParametersCollection requestParameters, string siteArea)
            : base(url, requestParameters, siteArea) { }

        //

        public NavigationRequest(string url, NavigateMode navigationMode)
            : this(url, null, null, navigationMode) { }

        public NavigationRequest(string url, string siteArea, NavigateMode navigationMode)
            : this(url, null, siteArea, navigationMode) { }

        public NavigationRequest(string url, ParametersCollection requestParameters, NavigateMode navigationMode)
            : this(url, requestParameters, null, navigationMode) { }

        public NavigationRequest(string url, ParametersCollection requestParameters, string siteArea, NavigateMode navigationMode)
            : base(url, requestParameters, siteArea)
        {
            _navigateMode = navigationMode;
        }


        #endregion

        #region Properties

        public NavigateMode NavigationMode
        {
            get
            {
                return _navigateMode.GetValueOrDefault(NavigateMode.New);
            }
        }

        internal IDisposableToken Token { get; set; }

        #endregion

        #region Methods

        public NavigationRequest Clone()
        {
            //if (_navigateMode.HasValue)
            //{
            //    return new NavigationRequest(_navigateMode.Value);
            //}
            //else 
            //{
            return new NavigationRequest(this.RequestUrl, this.RequestParameters, this.SiteArea, _navigateMode.GetValueOrDefault(NavigateMode.New));
            //}
        }

        #endregion

    }
}
