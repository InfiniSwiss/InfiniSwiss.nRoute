using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Internal;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace nRoute.Behaviors
{
    public abstract partial class NavigationAdapterBehaviorBase<T>
        : INavigationHandler, INavigationContainer, ISupportRefreshNavigation, ISupportNavigationViewState
    {
        private const string CURRENT_PAGESTATE_INFO_KEY = "_AdapterCurrentPageInfo";

        #region INavigationHandler Members

        void INavigationHandler.ProcessRequest(NavigationRequest request, Action<bool> requestCallback)
        {
            OnProcessRequest(request, requestCallback);
        }

        async Task INavigationHandler.ProcessResponseAsync(NavigationResponse response)
        {
            await OnProcessResponseAsync(response);
        }

        #endregion

        #region Overriden Methods

        protected virtual void OnUrlChanged(string oldUrl, string newUrl)
        {
            var _currentUrl = CurrentNavigationRequest != null ? CurrentNavigationRequest.RequestUrl : null;
            if (!string.Equals(_currentUrl, newUrl, StringComparison.OrdinalIgnoreCase))
            {
                Navigate(newUrl);
            }
        }

        protected virtual void OnNavigate(NavigationRequest request)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentValue(string.IsNullOrEmpty(request.RequestUrl), "request", NAVIGATIONURL_NOTNULLOREMPTY);

            NavigationService.Navigate(request, this);
        }

        protected virtual void OnProcessRequest(NavigationRequest request, Action<bool> requestCallback)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentNotNull(requestCallback, "requestCallback");

            // if there is a pending request
            if (ActiveNavigationRequest != null)
            {
                NavigationService.CancelNavigation(ActiveNavigationRequest);
                ActiveNavigationRequest = null;
            }

            // we say we are navigating
            var _cancelEventArgs = new NavigatingCancelEventArgs(this, request);
            OnNavigating(_cancelEventArgs);

            // if not cancelled, then we go to our base class to check against the ISupportNavigation
            if (!_cancelEventArgs.Cancel)
            {
                NavigationService.ProcessNavigationLifecycle(request, this.CurrentNavigationLifecycleSupporter, this.CurrentNavigationViewLifecycleSupporter,
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
            }
            else
            {
                requestCallback(false);
            }
        }

        protected async virtual Task OnProcessResponseAsync(NavigationResponse response)
        {
            Guard.ArgumentNotNull(response, "response");

            // we won't handle it the response, if it is not the active request!
            if (!response.Request.Equals(ActiveNavigationRequest)) return;

            // we check if the navigation was successful or not
            if (response.Status != ResponseStatus.Success)
            {
                // we raise navigation failed and return - note for cancelled it returns to navigated state 
                var _eventArgs = new NavigationFailedEventArgs(this, response.Request, response.Status, response.Error);
                OnNavigatingFailed(_eventArgs);
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
            OnSetNavigationContent(response.Content);

            // we raise completed
            OnNavigationCompleted(new NavigatedEventArgs(this, response.Request));
        }

        protected virtual void OnRefresh()
        {
            if (CurrentNavigationRequest != null)
            {
                var _refreshRequest = new NavigationRequest(CurrentNavigationRequest.RequestUrl,
                    CurrentNavigationRequest.RequestParameters, CurrentNavigationRequest.SiteArea, NavigateMode.Refresh);
                NavigationService.Navigate(_refreshRequest, this);
            }
        }

        protected virtual void OnRestoreState(ParametersCollection state)
        {
            OnSetNavigationContent(null);            // remove the current content

            CurrentNavigationRequest = state.GetValueOrDefault<PageContentState>(CURRENT_PAGESTATE_INFO_KEY, default(PageContentState));
            Refresh();
        }

        protected virtual ParametersCollection OnSaveState()
        {
            if (CurrentNavigationRequest == null) return null;

            return new ParametersCollection()
            {
                new Parameter(CURRENT_PAGESTATE_INFO_KEY, (PageContentState)CurrentNavigationRequest)
            };
        }

        #endregion

        #region Helpers

        protected abstract void OnSetNavigationContent(Object content);

        #endregion

        #region Navigating Events Related

        protected virtual void OnNavigating(NavigatingCancelEventArgs e)
        {
            if (Navigating != null) Navigating(this, e);
            // we don't transition here as the navigation might have been cancelled
            //TransitionToNavigationState(Controls.NavigationState.Navigating);
        }

        protected virtual void OnNavigationCompleted(NavigatedEventArgs e)
        {
            if (Navigated != null) Navigated(this, e);
            TransitionToNavigationState(Controls.NavigationState.Navigated);
        }

        protected virtual void OnNavigatingFailed(NavigationFailedEventArgs e)
        {
            if (NavigationFailed != null) NavigationFailed(this, e);

            if (e.Status != ResponseStatus.Cancelled)
            {
                ShowFailedNavigationState(e.Request, e.Status, e.Error);
                TransitionToNavigationState(Controls.NavigationState.NavigationFailed);
            }
            else
            {
                TransitionToNavigationState(Controls.NavigationState.Navigated);
            }
        }

        #endregion

        #region ISupportRefreshNavigation Members

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

    }
}
