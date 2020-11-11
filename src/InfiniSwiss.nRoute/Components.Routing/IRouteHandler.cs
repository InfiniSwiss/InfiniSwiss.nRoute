using System;

namespace nRoute.Components.Routing
{
    public interface IRouteHandler
    {
        /// <summary>
        /// Handles the routing request, and returns a response.
        /// </summary>
        /// <remarks>
        /// - The IObservable only starts processing the response once an item subscribes to it. 
        /// </remarks>
        IObservable<IUrlResponse> GetResponse(IRoutingContext context);
    }
}
