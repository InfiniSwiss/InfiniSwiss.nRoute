using nRoute.Internal;
using System;

namespace nRoute.Components.Routing
{
    public class UrlRequest
         : IUrlRequest, IEquatable<UrlRequest>
    {

        #region Declarations

        private const string INVALID_ROUTE_URL = "The URL, it cannot start with a '/' or '~' character or contain a '?' character.";
        private readonly static Char[] TRIM_CHARS = new Char[] { '/' };

        private readonly string _requestUrl;
        private readonly ParametersCollection _requestParameters;
        private readonly string _siteArea;

        #endregion

        protected UrlRequest() { }

        public UrlRequest(string requestUrl)
            : this(requestUrl, null, null) { }

        public UrlRequest(string requestUrl, string siteArea)
            : this(requestUrl, null, siteArea) { }

        public UrlRequest(string requestUrl, ParametersCollection requestParameters)
            : this(requestUrl, requestParameters, null) { }

        public UrlRequest(string requestUrl, ParametersCollection requestParameters, string siteArea)
        {
            Guard.ArgumentNotNullOrWhiteSpace(requestUrl, "requestUrl");

            // we check the url -- note_, I've add the trims of ~ and /
            requestUrl = requestUrl.Trim().TrimStart("~".ToCharArray()).TrimStart("/".ToCharArray());

            Guard.ArgumentValue((requestUrl.StartsWith("~", StringComparison.Ordinal) ||
                requestUrl.StartsWith("/", StringComparison.Ordinal)) ||
                (requestUrl.IndexOf('?') != -1), "requestUrl", INVALID_ROUTE_URL);

            // we save
            _requestUrl = requestUrl;
            _requestParameters = requestParameters ?? new ParametersCollection();
            _siteArea = siteArea;
        }

        #region IUrlRequest Members

        public virtual string RequestUrl
        {
            get
            {
                return _requestUrl;
            }
        }

        public virtual ParametersCollection RequestParameters
        {
            get
            {
                return _requestParameters;
            }
        }

        public virtual string SiteArea
        {
            get { return _siteArea; }
        }

        public virtual object ServiceState { get; set; }

        #endregion

        #region IEquatable<UrlRequest> Members

        public bool Equals(UrlRequest other)
        {
            if (other == null) return false;
            return string.Equals(RequestUrl.Trim(TRIM_CHARS), other.RequestUrl.Trim(TRIM_CHARS), StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}
