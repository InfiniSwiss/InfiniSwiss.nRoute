using nRoute.Components.Routing;
using System;

namespace nRoute.Controllers
{
    [Serializable]
    public class ControllerActionException
        : Exception
    {
        private readonly ControllerActionRequest _request;
        private readonly ResponseStatus _status;

        public ControllerActionException(string message, ResponseStatus status)
            : this(message, status, null, null) { }

        public ControllerActionException(string message, ResponseStatus status, ControllerActionRequest request)
            : this(message, status, request, null) { }

        public ControllerActionException(string message, ResponseStatus status, ControllerActionRequest request, Exception innerException)
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

        public ControllerActionRequest Request
        {
            get { return _request; }
        }

        #endregion

    }
}
