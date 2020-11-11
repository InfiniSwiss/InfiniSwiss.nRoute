using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Internal;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace nRoute.Controls
{
    public partial class NavigationContentControl
         : INavigationHandler, ISupportRefreshNavigation, ISupportNavigationViewState
    {
        private const string NAVIGATIONURL_NOTNULLOREMPTY = "Request's navigation url cannot be null or empty.";
        private const string CURRENT_PAGESTATE_INFO_KEY = "_CurrentPageInfo";
        private Queue<NavigationResponse> processResponseQueue = new Queue<NavigationResponse>();

        #region INavigationHandler Related

        void INavigationHandler.ProcessRequest(NavigationRequest request, Action<bool> requestCallback)
        {
            OnProcessRequest(request, requestCallback);
        }

        async Task INavigationHandler.ProcessResponseAsync(NavigationResponse response)
        {
            // It's possible that when processing the navigation response a new navigation is initiated
            // in the same handler (e.g. the navigation supporter/view model, while initializing triggers
            // a new navigation). If that's the case, we want to let the current navigation request to finish
            // processing before we process the new one, otherwise the control's internal state might get 
            // messed up (because the ProcessResponseImplAsync usually sets some class properties).
            //
            // To do this we enqueue the requests and only start processing the queue if it's the first
            // request, so that we don't process a request multiple times. If processing a request triggers
            // a new navigation this method will be called again, a new item will be added to the queue,
            // it won't be processed in this stack frame, but will be processed by the initial stack frame after
            // it's done awaiting for the in-flight navigation to finish.
            this.processResponseQueue.Enqueue(response);

            if (this.processResponseQueue.Count == 1)
            {
                while (this.processResponseQueue.Any())
                {
                    var _nextResponseToProcess = this.processResponseQueue.Peek();

                    await ProcessResponseImplAsync(_nextResponseToProcess);

                    this.processResponseQueue.Dequeue();
                }
            }
        }

        #endregion

        #region ISupportRefreshNavigation Related

        public bool CanRefresh
        {
            get { return (CurrentNavigationRequest != null); }
        }

        public void Refresh()
        {
            OnRefresh();
        }

        #endregion

        #region ISupportNavigationViewState

        void ISupportNavigationViewState.RestoreState(ParametersCollection state)
        {
            OnRestoreState(state);
        }

        ParametersCollection ISupportNavigationViewState.SaveState()
        {
            return OnSaveState();
        }

        #endregion

        #region Overriden Methods

        protected virtual void OnUrlChanged(string oldUrl, string newUrl)
        {
            if (string.IsNullOrEmpty(newUrl)) return;
            var _currentUrl = CurrentNavigationRequest != null ? CurrentNavigationRequest.RequestUrl : null;

            // if our requested url is not equal to our current url then navigate
            if (!string.Equals(_currentUrl, newUrl, StringComparison.OrdinalIgnoreCase))
            {
                Navigate(newUrl);
            }
        }

        protected virtual void OnNavigate(NavigationRequest request)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentValue(string.IsNullOrEmpty(request.RequestUrl), "request", NAVIGATIONURL_NOTNULLOREMPTY);

            Navigation.NavigationService.Navigate(request, this);
        }

        protected virtual void OnProcessRequest(NavigationRequest request, Action<bool> requestCallback)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentNotNull(requestCallback, "requestCallback");

            // if there is a pending request
            if (ActiveNavigationRequest != null)
            {
                Navigation.NavigationService.CancelNavigation(ActiveNavigationRequest);
                ActiveNavigationRequest = null;
            }

            NavigationService.ProcessNavigationLifecycle(request, CurrentNavigationLifecycleSupporter, CurrentNavigationViewLifecycleSupporter,
                (b) =>
                {
                    if (b)
                    {
                        ActiveNavigationRequest = request;
                    }
                    else
                    {
                        requestCallback(b);
                    }
                },
                () =>
                {
                    TransitionToNavigationState(Controls.NavigationState.Navigating);
                    requestCallback(true);
                });

            //// we check if our current page supports ISupportNavigation, if so we defer to it
            //if (CurrentNavigationLifecycleSupporter != null)
            //{
            //    CurrentNavigationLifecycleSupporter.Closing((b) =>
            //    {
            //        if (b)
            //        {
            //            ActiveNavigationRequest = request;
            //            if (CurrentNavigationViewLifecycleSupporter != null)
            //            {
            //                CurrentNavigationViewLifecycleSupporter.Closing(request, () =>
            //                {
            //                    TransitionToNavigationState(Controls.NavigationState.Navigating);
            //                    requestCallback(b);
            //                });
            //            }
            //            else
            //            {
            //                TransitionToNavigationState(Controls.NavigationState.Navigating);
            //                requestCallback(b);
            //            }
            //        }
            //        else
            //        {
            //            requestCallback(b);
            //        }
            //    });
            //}
            //else
            //{
            //    ActiveNavigationRequest = request;
            //    if (CurrentNavigationViewLifecycleSupporter != null)
            //    {
            //        CurrentNavigationViewLifecycleSupporter.Closing(request, () =>
            //        {
            //            TransitionToNavigationState(Controls.NavigationState.Navigating);
            //            requestCallback(true);
            //        });
            //    }
            //    else
            //    {
            //        TransitionToNavigationState(Controls.NavigationState.Navigating);
            //        requestCallback(true);
            //    }
            //}
        }

        protected async virtual Task ProcessResponseImplAsync(NavigationResponse response)
        {
            Guard.ArgumentNotNull(response, "response");

            // we won't handle it the response, if it is not the active request!
            if (!response.Request.Equals(ActiveNavigationRequest)) return;

            // we check if the navigation was successful or not
            if (response.Status != ResponseStatus.Success)
            {
                // if cancelled then return to normal state, else we inform the error 
                if (response.Status == ResponseStatus.Cancelled)
                {
                    TransitionToNavigationState(Controls.NavigationState.Navigated);
                    return;
                }
                ShowFailedNavigationState(response.Request, response.Status, response.Error);
                return;
            }

            // we save our supporters and use them - note_ we need to save this to access later
            CurrentNavigationLifecycleSupporter = NavigationService.GetSupporter<ISupportNavigationLifecycle>(response.Content);
            CurrentNavigationViewLifecycleSupporter = NavigationService.GetSupporter<ISupportNavigationViewLifecycle>(response.Content);
            CurrentNavigationStateSupporter = NavigationService.GetSupporter<ISupportNavigationState>(response.Content);
            CurrentNavigationViewStateSupporter = NavigationService.GetSupporter<ISupportNavigationViewState>(response.Content);

            // if the content support navigation, we pass in the request/response merged parameters
            if (this.CurrentNavigationLifecycleSupporter != null)
            {
                CurrentNavigationLifecycleSupporter.Initialize(response.ResponseParameters);
                await CurrentNavigationLifecycleSupporter.InitializeAsync(response.ResponseParameters);
                Title = CurrentNavigationLifecycleSupporter.Title ??
                    nRoute.Navigation.Navigation.GetTitle(response.Content as DependencyObject);
            }
            else
            {
                Title = nRoute.Navigation.Navigation.GetTitle(response.Content as DependencyObject);
            }

            // we save the url and the content
            CurrentNavigationRequest = response.Request;
            SetValue(ContentProperty, response.Content);
            TransitionToNavigationState(Controls.NavigationState.Navigated);

            // and once we have set the transition state, then we call the view's initialize
            if (CurrentNavigationViewLifecycleSupporter != null) CurrentNavigationViewLifecycleSupporter.Initialize(response);
        }

        protected virtual void OnRefresh()
        {
            if (CurrentNavigationRequest != null)
            {
                var _refreshRequest = new NavigationRequest(CurrentNavigationRequest.RequestUrl,
                    CurrentNavigationRequest.RequestParameters, CurrentNavigationRequest.SiteArea, NavigateMode.Refresh);
                Navigation.NavigationService.Navigate(_refreshRequest, this);
            }
        }

        protected virtual void OnRestoreState(ParametersCollection state)
        {
            base.Content = null;            // remove the current content

            CurrentNavigationRequest = state.GetValueOrDefault<PageContentState>(CURRENT_PAGESTATE_INFO_KEY, default(PageContentState));
            Refresh();
        }

        protected virtual ParametersCollection OnSaveState()
        {
            if (CurrentNavigationRequest == null) return null;

            // note we save as a PageContentState because that is serializable and NavigationRequest isn't for SL
            return new ParametersCollection()
            {
                new Parameter(CURRENT_PAGESTATE_INFO_KEY, (PageContentState)CurrentNavigationRequest)
            };
        }

        #endregion

    }
}