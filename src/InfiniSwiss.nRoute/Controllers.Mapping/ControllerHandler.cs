using nRoute.Components;
using nRoute.Components.Composition;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Controllers.Mapping
{
    public class ControllerHandler
        : IRouteHandler
    {
        private readonly Type _controllerType;

        public ControllerHandler(Type controllerType)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            Guard.ArgumentValue(!typeof(IController).IsAssignableFrom(controllerType), "controllerType");

            _controllerType = controllerType;
        }

        #region Main Methods

        public IObservable<IUrlResponse> GetResponse(IRoutingContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentIsType(context.Request, typeof(ControllerActionRequest), "context.Request");

            var _relayObservable = new LazyRelayObservable<IUrlResponse>((s) =>
            {
                var _request = (ControllerActionRequest)context.Request;
                var _controller = (IController)TypeBuilder.BuildType(_controllerType);
                var _response = new ControllerContext(_request, ResponseStatus.Success, _controller, context.ParsedParameters);
                s.OnNext(_response);
                s.OnCompleted();
            });

            return _relayObservable;
        }

        #endregion

        #region IRouteHandler Members

        IObservable<IUrlResponse> IRouteHandler.GetResponse(IRoutingContext context)
        {
            return GetResponse(context);
        }

        #endregion

    }
}
