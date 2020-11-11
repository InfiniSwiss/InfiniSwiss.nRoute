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
    public partial class BrowsingContainer
         : IBrowsingContainer
    {
        private const string BACK_STACK_KEY = "_BrowsingBackStack";
        private const string FORWARD_STACK_KEY = "_BrowsingForwardStack";
        private const string INTERMEDIATE_PAGESTATE_KEY = "_BrowsingIntermediateState";

        private Stack<PageContentState> _backStack;
        private Stack<PageContentState> _forwardStack;
        private PageContentState _intermediateState;

        #region Constructor

        public BrowsingContainer()
        {
            _backStack = new Stack<PageContentState>();
            _forwardStack = new Stack<PageContentState>();
        }

        #endregion

        #region IBrowsingContainer Members

        public event EventHandler<NavigationContainerEventArgs> JournalPurged;

        public void NavigateBack(IEnumerable<string> toAnyOfUrls = null)
        {
            OnNavigateBack(toAnyOfUrls);
        }

        public void NavigateForward()
        {
            OnNavigateForward();
        }

        public bool CanNavigateBack
        {
            get { return OnCanNavigateBack(); }
        }

        public bool CanNavigateForward
        {
            get { return OnCanNavigateForward(); }
        }

        public bool CanPurgeJournal
        {
            get { return (OnCanNavigateBack() || OnCanNavigateForward()); }
        }

        public void PurgeJournal()
        {
            OnPurgeJournals();
        }

        public IEnumerable<PageContentState> BackPagesJournal
        {
            get
            {
                return _backStack;
            }
        }

        public IEnumerable<PageContentState> ForwardPagesJournal
        {
            get
            {
                return _forwardStack;
            }
        }

        #endregion

        #region Overriden Methods
        protected override async Task ProcessResponseImplAsync(NavigationResponse response)
        {
            Guard.ArgumentNotNull(response, "response");

            // we won't handle it the response, if it is not the active request!
            if (!response.Request.Equals(base.ActiveNavigationRequest)) return;

            // we check if the navigation was successful or not
            if (response.Status != ResponseStatus.Success)
            {
                // we raise navigation failed and return
                await base.ProcessResponseImplAsync(response);
                return;
            }

            // note_ we don't send this request to the base class, hereon

            // we get the current page info, as in the current page before being navigated
            var _currentPageInfo = GetCurrentPageContentState();
            var _nextPageInfo = default(PageContentState);

            // else as per the navigation mode we change the stacks
            switch (response.Request.NavigationMode)
            {
                // we treat new and unknown alike
                case NavigateMode.New:
                case NavigateMode.Unknown:

                    // we check if we were to record
                    if (_currentPageInfo != null) _backStack.Push(_currentPageInfo);

                    // we also clear the forward stack
                    _forwardStack.Clear();

                    // and we say history changed
                    RaiseHistoryChanged();
                    break;

                case NavigateMode.Back:
                    if (CanNavigateBack)
                    {
                        // we push the current item into the forward stack
                        _forwardStack.Push(_currentPageInfo);

                        // we remove one item
                        _nextPageInfo = _backStack.Pop();

                        // and we say history changed
                        RaiseHistoryChanged();
                    }

                    break;

                case NavigateMode.Forward:

                    // we push the current item into the back stack
                    _backStack.Push(_currentPageInfo);

                    // and we remove one item
                    _nextPageInfo = _forwardStack.Pop();

                    // and we say history changed and return
                    RaiseHistoryChanged();
                    break;

                case NavigateMode.Refresh:

                    // we don't need to do anything here, as the history remains the same
                    _nextPageInfo = _currentPageInfo;
                    break;
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
                Title = CurrentNavigationLifecycleSupporter?.Title ??
                    nRoute.Navigation.Navigation.GetTitle(response.Content as DependencyObject);
            }
            else
            {
                Title = nRoute.Navigation.Navigation.GetTitle(response.Content as DependencyObject);
            }

            // if the content supports state, and we are navigating back/forward
            if (CurrentNavigationStateSupporter != null)
            {
                // if we are navigating back  or forward, and we are to - also we treat the unknown state to be like a New navigation
                if (response.Request.NavigationMode != NavigateMode.New) // && response.Request.NavigationMode != NavigateMode.Unknown)
                {
                    CurrentNavigationStateSupporter.RestoreState(_nextPageInfo != null ? _nextPageInfo.State : null);
                }
            }

            // if the content supports view state, and we are navigating back/forward
            if (CurrentNavigationViewStateSupporter != null)
            {
                // if we are navigating back  or forward, and we are to - also we treat the unknown state to be like a New navigation
                if (response.Request.NavigationMode != NavigateMode.New)
                {
                    CurrentNavigationViewStateSupporter.RestoreState(_nextPageInfo != null ? _nextPageInfo.VisualState : null);
                }
            }

            // we save the url and the content
            CurrentNavigationRequest = response.Request;
            SetValue(ContentProperty, response.Content);

            // and raise the event navigation completed
            OnNavigationCompleted(new NavigatedEventArgs(this, response.Request));

            // and once we have set the transition state, then we call the view's initialize
            if (CurrentNavigationViewLifecycleSupporter != null)
            {
                CurrentNavigationViewLifecycleSupporter.Initialize(response);
            }
        }

        protected override ParametersCollection OnSaveState()
        {
            var _parameters = base.OnSaveState();
            if (_parameters == null) _parameters = new ParametersCollection();

            if (_backStack.Count > 0 || _forwardStack.Count > 0)
            {
                // note we store an array, and not a stack - as we shouldn't store a reference
                _parameters.Add(BACK_STACK_KEY, _backStack.Reverse().ToArray());
                _parameters.Add(FORWARD_STACK_KEY, _forwardStack.Reverse().ToArray());
            }

            if (this.Url != null)
            {
                _parameters.Add(INTERMEDIATE_PAGESTATE_KEY, GetCurrentPageContentState());
            }

            return _parameters;
        }

        protected override void OnRestoreState(ParametersCollection state)
        {
            _backStack = new Stack<PageContentState>(state.GetValueOrDefault(BACK_STACK_KEY, Enumerable.Empty<PageContentState>()));
            _forwardStack = new Stack<PageContentState>(state.GetValueOrDefault(FORWARD_STACK_KEY, Enumerable.Empty<PageContentState>()));
            _intermediateState = state.GetValueOrDefault(INTERMEDIATE_PAGESTATE_KEY, default(PageContentState));

            base.OnRestoreState(state);
            RaiseHistoryChanged();
        }

        #endregion

        #region Browsing Related Methods

        protected virtual void OnNavigateBack(IEnumerable<string> toAnyOfUrls = null)
        {
            if (!CanNavigateBack) return;

            // we get the request
            var _backPageInfo = _backStack.Peek();

            if (toAnyOfUrls != null)
            {
                while (!toAnyOfUrls.Contains(_backPageInfo.Url))
                {
                    this._backStack.Pop();

                    if (_backStack.Count == 0)
                    {
                        throw new Exception("Navigation target not found");
                    }

                    _backPageInfo = _backStack.Peek();
                }
            }

            var _request = new NavigationRequest(_backPageInfo.Url, _backPageInfo.State, NavigateMode.Back);

            // and we navigate, if the thing is not cancelled then
            // NOTE: Currently the navigate is synchronous so their are implications
            Navigation.NavigationService.Navigate(_request, this);
        }

        protected virtual void OnNavigateForward()
        {
            if (!CanNavigateForward) return;

            // we get the request
            var _forwardPageInfo = _forwardStack.Peek();
            var _request = new NavigationRequest(_forwardPageInfo.Url, _forwardPageInfo.State, NavigateMode.Forward);

            // and we navigate, if the thing is not cancelled then
            // NOTE: Currently the navigate is synchronous so their are implications
            Navigation.NavigationService.Navigate(_request, this);
        }

        protected virtual bool OnCanNavigateBack()
        {
            return (_backStack.Count > 0);
        }

        protected virtual bool OnCanNavigateForward()
        {
            return (_forwardStack.Count > 0);
        }

        protected virtual void OnPurgeJournals()
        {
            if (!CanNavigateBack && !CanNavigateForward) return;

            // we clear
            _backStack.Clear();
            _forwardStack.Clear();

            // and we say history changed
            RaiseHistoryChanged();

            // we raise the event
            OnJournalsPurged(new NavigationContainerEventArgs(this));
        }

        #endregion

        #region Navigating Events Related

        protected virtual void OnJournalsPurged(NavigationContainerEventArgs e)
        {
            if (JournalPurged != null) JournalPurged(this, e);
        }

        #endregion

        #region Helpers

        private PageContentState GetCurrentPageContentState()
        {
            // we check if an intermediate state is available
            if (_intermediateState != null)
            {
                var _state = _intermediateState;
                _intermediateState = null;           // reset it
                return _state;
            }

            // we check the page has a url
            if (string.IsNullOrEmpty(this.Url)) return null;

            // we also add
            return new PageContentState(this.Url, this.CurrentNavigationRequest != null ? this.CurrentNavigationRequest.SiteArea : null, this.Title,
                (CurrentNavigationStateSupporter == null ? null : CurrentNavigationStateSupporter.SaveState()),
                (CurrentNavigationViewStateSupporter == null) ? null : CurrentNavigationViewStateSupporter.SaveState());
        }

        private void RaiseHistoryChanged()
        {
            base.NotifyPropertyChanged(() => CanNavigateBack);
            base.NotifyPropertyChanged(() => CanNavigateForward);
            base.NotifyPropertyChanged(() => CanPurgeJournal);
        }

        #endregion

    }
}
