using nRoute.Components;
using nRoute.Components.Routing;
using System;

namespace nRoute.Controllers
{
    public class ControllerContext
        : UrlResponse
    {
        public ControllerContext(ControllerActionRequest request, ResponseStatus responseStatus, IController controller,
            ParametersCollection responseParameters)
            : base(request, responseStatus, controller, responseParameters) { }

        public ControllerContext(ControllerActionRequest request, ResponseStatus responseStatus, IController controller,
            ParametersCollection responseParameters, Exception error)
            : base(request, responseStatus, controller, responseParameters, error) { }

        /// <remarks>This is to wrap any non-success reply into a action response</remarks>
        internal ControllerContext(IUrlResponse response)
            : this((ControllerActionRequest)response.Request, response.Status, response.Content as IController,
            response.ResponseParameters, response.Error)
        { }

        #region Properties

        public IController Controller
        {
            get { return (IController)base.Content; }
        }

        public new ControllerActionRequest Request
        {
            get { return (ControllerActionRequest)base.Request; }
        }

        #endregion

    }
}
