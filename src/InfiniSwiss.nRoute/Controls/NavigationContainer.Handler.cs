using nRoute.Components.Routing;
using nRoute.Internal;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace nRoute.Controls
{
    public partial class NavigationContainer
         : INavigationContainer
    {

        #region INavigationContainer Members

        public event EventHandler<NavigatingCancelEventArgs> Navigating;

        public event EventHandler<NavigatedEventArgs> Navigated;

        public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        // the rest are inherited from the base class

        #endregion

        #region Main Overridable Methods

        protected override void OnProcessRequest(NavigationRequest request, Action<bool> requestCallback)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentNotNull(requestCallback, "requestCallback");

            // we say we are navigating
            var _cancelEventArgs = new NavigatingCancelEventArgs(this, request);
            OnNavigating(_cancelEventArgs);

            // if not cancelled, then we go to our base class to check against the ISupportNavigation
            if (!_cancelEventArgs.Cancel)
            {
                base.OnProcessRequest(request, requestCallback);
            }
            else
            {
                requestCallback(false);
            }
        }

        protected async override Task ProcessResponseImplAsync(NavigationResponse response)
        {
            Guard.ArgumentNotNull(response, "response");

            // we won't handle it the response, if it is not the active request!
            if (!response.Request.Equals(base.ActiveNavigationRequest)) return;

            // we check if the navigation was successful or not
            if (response.Status != ResponseStatus.Success)
            {
                // we raise navigation failed and return - note for cancelled it returns to navigated state 
                var _failedEventArgs = new NavigationFailedEventArgs(this, response.Request, response.Status, response.Error);
                OnNavigatingFailed(_failedEventArgs);
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

            // we raise completed
            OnNavigationCompleted(new NavigatedEventArgs(this, response.Request));

            // and once we have set the transition state, then we call the view's initialize
            if (CurrentNavigationViewLifecycleSupporter != null) CurrentNavigationViewLifecycleSupporter.Initialize(response);
        }

        #endregion

        #region Navigating Events Related

        protected virtual void OnNavigating(NavigatingCancelEventArgs e)
        {
            // we raise the event
            if (Navigating != null) Navigating(this, e);

            // - WE DON't Transition here as the thing might have been cancelled
            //TransitionToNavigationState(NavigationState.Navigating, true);
        }

        protected virtual void OnNavigationCompleted(NavigatedEventArgs e)
        {
            // we raise the event
            if (Navigated != null) Navigated(this, e);

            // and indicate the visual state
            TransitionToNavigationState(NavigationState.Navigated, true);
        }

        protected virtual void OnNavigatingFailed(NavigationFailedEventArgs e)
        {
            // we raise the event
            if (NavigationFailed != null) NavigationFailed(this, e);

            // and indicate the visual state
            if (e.Status != ResponseStatus.Cancelled)
            {
                ShowFailedNavigationState(e.Request, e.Status, e.Error);
                TransitionToNavigationState(NavigationState.NavigationFailed, true);
            }
            else
            {
                TransitionToNavigationState(Controls.NavigationState.Navigated);
            }
        }

        #endregion

    }
}
