using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Internal;
using System;

namespace nRoute.Navigation
{
    public static partial class NavigationService
    {
        private const string AREA_COULDNOT_INITIALIZE = "Could not initialize Site Area '{0}' to navigate";
        private const string NO_DEFAULT_HANDLERFOUND = "No application-wide default navigation handler found, please specify handler";

        #region Resolve Related

        public static void Resolve(NavigationRequest navigationRequest, Action<NavigationResponse> responseCallback)
        {
            Guard.ArgumentNotNull(navigationRequest, "navigationRequest");
            Guard.ArgumentNotNull(responseCallback, "responseCallback");

            // NOTE_: if the response is a navigation response, we send that else we wrap it in a navigation response
            // this is done because the api expects this.. we need this though, because if an error occurs we will 
            // get back UrlResponse not NavigationResponse.. so..
            RoutingService.Resolve(navigationRequest, (r) =>
            {
                if (typeof(NavigationResponse).IsAssignableFrom(r.GetType()))
                {
                    responseCallback((NavigationResponse)r);
                }
                else
                {
                    responseCallback(new NavigationResponse(r));
                }
            });
        }

        public static bool CancelNavigation(NavigationRequest navigationRequest)
        {
            Guard.ArgumentNotNull(navigationRequest, "navigationRequest");
            if (navigationRequest.Token == null || navigationRequest.Token.IsDisposed)
            {
                return false;
            }
            else
            {
                navigationRequest.Token.Dispose();
                return true;
            }
        }

        #endregion

        #region Navigation Related

        public static IDisposable Navigate(NavigationRequest navigationRequest)
        {
            return Navigate(navigationRequest, (Action<ResponseStatus>)null);
        }

        public static IDisposable Navigate(NavigationRequest navigationRequest, Action<ResponseStatus> statusCallback)
        {
            Guard.ArgumentNotNull(navigationRequest, "navigationRequest");

            var _defaultHandler = NavigationService.DefaultNavigationHandler;
            if (_defaultHandler == null)
            {
                statusCallback.Notify(ResponseStatus.HandlerNotFound);
                throw new NavigationException(NO_DEFAULT_HANDLERFOUND, ResponseStatus.HandlerNotFound, navigationRequest);
            }

            return Navigate(navigationRequest, NavigationService.DefaultNavigationHandler, statusCallback);
        }

        public static IDisposable Navigate(NavigationRequest navigationRequest, INavigationHandler navigationHandler)
        {
            return Navigate(navigationRequest, navigationHandler, null);
        }

        public static IDisposable Navigate(NavigationRequest navigationRequest, INavigationHandler navigationHandler, Action<ResponseStatus> statusCallback)
        {
            Guard.ArgumentNotNull(navigationRequest, "navigationRequest");
            Guard.ArgumentNotNull(navigationHandler, "navigationHandler");

            // if the request already had a token then dispose it
            if (navigationRequest.Token != null)
            {
                navigationRequest.Token.Dispose();
            }

            // create a new token
            var _disposableToken = new DisposableToken();
            navigationRequest.Token = _disposableToken;

            // and process the response
            navigationHandler.ProcessRequest(navigationRequest, async (b) =>
            {
                // if the handler will not process it - note we don't return the 
                if (!b)
                {
                    statusCallback.Notify(ResponseStatus.Cancelled);
                    _disposableToken.Dispose();
                    return;
                }

                // if the token was already disposed then we tell the handler it was cancelled
                if (_disposableToken.IsDisposed)
                {
                    statusCallback.Notify(ResponseStatus.Cancelled);
                    await navigationHandler.ProcessResponseAsync(new NavigationResponse(navigationRequest, ResponseStatus.Cancelled, null, null));
                    return;
                }

                // we delegate to the routing engine
                Resolve(navigationRequest, async (r) =>
                {
                    // if not cancelled
                    if (!_disposableToken.IsDisposed)
                    {
                        await navigationHandler.ProcessResponseAsync(r);
                        statusCallback.Notify(r.Status);
                    }
                    else
                    {
                        await navigationHandler.ProcessResponseAsync(new NavigationResponse(navigationRequest, ResponseStatus.Cancelled, null, null));
                        statusCallback.Notify(ResponseStatus.Cancelled);
                    }
                });

            });

            return _disposableToken;
        }

        #endregion

        #region Lifecycle related

        public static void ProcessNavigationLifecycle(NavigationRequest request, ISupportNavigationLifecycle supporter,
            ISupportNavigationViewLifecycle viewSupporter, Action<bool> supporterCallback, Action viewSupporterCallback)
        {
            // we check if our current page supports ISupportNavigation, if so we defer to it
            if (supporter != null)
            {
                supporter.Closing((b) =>
                {
                    supporterCallback(b);
                    if (b)
                    {
                        if (viewSupporter != null)
                        {
                            viewSupporter.Closing(request, () =>
                            {
                                viewSupporterCallback();
                            });
                        }
                        else
                        {
                            viewSupporterCallback();
                        }
                    }
                });
            }
            else
            {
                supporterCallback(true);
                if (viewSupporter != null)
                {
                    viewSupporter.Closing(request, () =>
                    {
                        viewSupporterCallback();
                    });
                }
                else
                {
                    viewSupporterCallback();
                }
            }
        }

        #endregion

        #region Helpers

        private static void Notify(this Action<ResponseStatus> statusCallback, ResponseStatus responseStatus)
        {
            if (statusCallback != null) statusCallback(responseStatus);
        }

        #endregion

    }
}