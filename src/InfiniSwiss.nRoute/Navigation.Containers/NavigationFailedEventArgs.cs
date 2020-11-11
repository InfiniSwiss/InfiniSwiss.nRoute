using nRoute.Components.Routing;
using System;

namespace nRoute.Navigation.Containers
{
    public class NavigationFailedEventArgs
         : NavigationContainerEventArgs
    {
        ResponseStatus _status;
        NavigationRequest _request;
        Exception _error;

        public NavigationFailedEventArgs(INavigationContainer container, NavigationRequest request,
            ResponseStatus status, Exception exception)
         : base(container)
        {
            _request = request;
            _status = status;
            _error = exception;
        }

        #region Properties

        public NavigationRequest Request
        {
            get
            {
                return _request;
            }
        }

        public Exception Error
        {
            get
            {
                return _error;
            }
        }

        public ResponseStatus Status
        {
            get
            {
                return _status;
            }
        }

        #endregion

    }
}
