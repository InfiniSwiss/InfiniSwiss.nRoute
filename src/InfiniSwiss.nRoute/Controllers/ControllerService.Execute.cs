using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Controllers
{
    public static partial class ControllerService
    {
        private const string ACTION_COULDNOT_EXECUTE = "Could not execute controller action for Url '{0}'";

        #region Resolve Related

        public static void Resolve(ControllerActionRequest actionRequest, Action<ControllerContext> responseCallback)
        {
            Guard.ArgumentNotNull(actionRequest, "request");
            Guard.ArgumentNotNull(responseCallback, "responseCallback");

            // NOTE_: if the response is a action context response, we send that else we wrap it in a action 
            // context response this is done because the api expects this.. 
            RoutingService.Resolve(actionRequest, (r) =>
            {
                if (typeof(ControllerContext).IsAssignableFrom(r.GetType()))
                {
                    responseCallback((ControllerContext)r);
                }
                else
                {
                    responseCallback(new ControllerContext(r));
                }
            });
        }

        #endregion

        #region Execute Related

        public static void Execute(ControllerActionRequest actionRequest)
        {
            Execute(actionRequest, null, null);
        }

        public static void Execute(ControllerActionRequest actionRequest, Action<ResponseStatus> statusCallback)
        {
            Execute(actionRequest, null, statusCallback);
        }

        public static void Execute(ControllerActionRequest actionRequest, Action<ControllerContext> contextCallback)
        {
            Execute(actionRequest, contextCallback, null);
        }

        public static void Execute(ControllerActionRequest actionRequest, Action<ControllerContext> contextCallback, Action<ResponseStatus> statusCallback)
        {
            Guard.ArgumentNotNull(actionRequest, "actionRequest");

            Resolve(actionRequest, (r) =>
            {
                contextCallback?.Invoke(r);
                statusCallback?.Invoke(r.Status);
                if (r.Controller != null)
                {
                    r.Controller?.Execute(r);
                }
                else if (r.Status != ResponseStatus.Success && r.Status != ResponseStatus.Cancelled)
                {
                    throw new ControllerActionException(string.Format(ACTION_COULDNOT_EXECUTE, actionRequest.RequestUrl),
                        r.Status, actionRequest, r.Error);
                }
            });
        }

        #endregion

    }
}