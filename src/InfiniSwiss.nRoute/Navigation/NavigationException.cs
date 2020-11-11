using nRoute.Components.Routing;
using System;

namespace nRoute.Navigation
{
#if (!SILVERLIGHT)
    [Serializable]
#endif
    public class NavigationException
        : Exception
    {
        private readonly NavigationRequest _request;
        private readonly ResponseStatus _status;

        public NavigationException(string message, ResponseStatus status)
            : this(message, status, null, null) { }

        public NavigationException(string message, ResponseStatus status, NavigationRequest request)
            : this(message, status, request, null) { }

        public NavigationException(string message, ResponseStatus status, NavigationRequest request, Exception innerException)
            : base(message, innerException)
        {
            _status = status;
            _request = request;
        }

        #region Properties

        public ResponseStatus Status
        {
            get { return _status; }
        }

        public NavigationRequest Request
        {
            get { return _request; }
        }

        #endregion

    }
}
