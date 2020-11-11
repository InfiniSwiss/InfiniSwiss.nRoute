using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Navigation.Mapping
{
    public class NavigationContentHandler
         : IRouteHandler
    {

        #region Constants and Variables

        private const string NO_PAGE_RESOLVED = "ResponseResolver Func did not return a non-null value for '{0}' navigation request.";
        private const string REQUEST_TYPE_REQUIRED = "NavigationRouteHandler can only respond to type NavigationRequest requests.";

        private readonly Func<ParametersCollection, Object> _responseResolver;

        #endregion

        public NavigationContentHandler(Func<ParametersCollection, Object> responseResolver)
        {
            Guard.ArgumentNotNull(responseResolver, "responseResolver");
            _responseResolver = responseResolver;
        }

        #region Main Methods

        public IObservable<IUrlResponse> GetResponse(IRoutingContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentIsType(context.Request, typeof(NavigationRequest), "context.Request");

            // we initialize a lazy observer that is initialized when subscribed too
            LazyRelayObservable<IUrlResponse> _relayObservable = null;
            _relayObservable = new LazyRelayObservable<IUrlResponse>((o) =>
            {
                // we get the response content, if it is empty we return
                Object _content = _responseResolver(context.ParsedParameters);
                if (_content == null)
                {
                    var _response = new NavigationResponse((NavigationRequest)context.Request,
                        ResponseStatus.UrlNotFound, _content, context.ParsedParameters);
                    o.OnNext(_response);

                    // need to specialize this exception
                    //_localObservable.OnError(new InvalidOperationException(string.Format(NO_PAGE_RESOLVED, context.Request.Url)));
                }
                else
                {
                    // we get the navigation response
                    var _response = new NavigationResponse((NavigationRequest)context.Request,
                        ResponseStatus.Success, _content, context.ParsedParameters);
                    _relayObservable.OnNext(_response);
                    _relayObservable.OnCompleted();

                }
            });

            // and return
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