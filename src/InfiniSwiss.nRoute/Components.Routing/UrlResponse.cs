using nRoute.Internal;
using System;

namespace nRoute.Components.Routing
{
    public class UrlResponse
         : IUrlResponse
    {
        private readonly IUrlRequest _request;
        private readonly Object _content;
        private readonly ResponseStatus _responseStatus;
        private readonly Exception _error;
        private readonly ParametersCollection _responseParameters;

        #region Constructors

        public UrlResponse(IUrlRequest request, ResponseStatus responseStatus,
            Object content, ParametersCollection responseParameters)
            : this(request, responseStatus, content, responseParameters, null) { }

        public UrlResponse(IUrlRequest request, ResponseStatus responseStatus, Exception error)
            : this(request, responseStatus, null, null, error) { }

        internal UrlResponse(IUrlRequest request, ResponseStatus responseStatus,
            Object content, ParametersCollection responseParameters, Exception error)
        {
            Guard.ArgumentNotNull(request, "request");

            _request = request;
            _responseStatus = responseStatus;
            _content = content;
            _responseParameters = responseParameters;
            _error = error;
        }

        #endregion

        #region IRoutingResponse Members

        public virtual IUrlRequest Request
        {
            get { return _request; }
        }

        public virtual ParametersCollection ResponseParameters
        {
            get { return _responseParameters; }
        }

        public virtual object Content
        {
            get { return _content; }
        }

        public ResponseStatus Status
        {
            get { return _responseStatus; }
        }

        public Exception Error
        {
            get { return _error; }
        }

        #endregion

    }
}
