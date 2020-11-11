using nRoute.Components;
using nRoute.Components.Routing;
using System;

namespace nRoute.Navigation
{
    public class NavigationResponse
         : UrlResponse
    {
        public NavigationResponse(NavigationRequest request, ResponseStatus status, Object content,
            ParametersCollection responseParameters)
         : this(request, status, content, responseParameters, null) { }

        public NavigationResponse(NavigationRequest request, ResponseStatus status, Object content,
            ParametersCollection responseParameters, Exception exception)
         : base(request, status, content, responseParameters, exception) { }

        /// <remarks>This is to wrap any non-successful reply into a navigation response, this
        /// is required because the Navigation Service API only returns a NavigationResponse type</remarks>
        internal NavigationResponse(IUrlResponse response)
         : this((NavigationRequest)response.Request,
            response.Status, response.Content, response.ResponseParameters, response.Error)
        { }

        #region Properties

        //public ISupportNavigationLifecycle NavigationLifecycleSupporter { get; set; }

        //public ISupportNavigationState NavigationStateSupporter { get; set; }

        public new NavigationRequest Request
        {
            get { return (NavigationRequest)base.Request; }
        }

        #endregion

    }
}