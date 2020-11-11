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
    public partial class StatefulBrowsingContainer
         : IBrowsingContainer, IStatefulContainer
    {
        private const string BACK_STACK_KEY = "_StatefulBackStack";
        private const string FORWARD_STACK_KEY = "_StatefulForwardStack";
        private const string STATES_DICTIONAY_KEY = "_StatefulStates";

        private Stack<PageContentState> _backStack;
        private Stack<PageContentState> _forwardStack;
        private Dictionary<string, PageContentState> _states;
        private bool _isRestoring;

        #region Constructor

        public StatefulBrowsingContainer()
        {
            _backStack = new Stack<PageContentState>();
            _forwardStack = new Stack<PageContentState>();
            _states = new Dictionary<string, PageContentState>(StringComparer.InvariantCultureIgnoreCase);
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
            get { return (OnCanNavigateBack() || OnCanNavigateForward() || (_states != null && _states.Count > 0)); }
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

        #region IStatefulContainer Members

        public IEnumerable<PageContentState> NavigatedPagesJournal
        {
            get { return _states.Values; }
        }

        public bool HasState(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return _states.ContainsKey(url);
        }

        #endregion

        #region Overriden Methods

        protected async override Task ProcessResponseImplAsync(NavigationResponse response)
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

            // we get the current page info
            var _currentPageStatelessInfo = GetCurrentPageStatelessContentState();

            // else as per the navigation mode we change the stacks
            switch (response.Request.NavigationMode)
            {
                // we treat new and unknown alike
                case NavigateMode.New:
                case NavigateMode.Unknown:

                    // we check if we were to record
                    if (_currentPageStatelessInfo != null) _backStack.Push(_currentPageStatelessInfo);

                    // we also clear the forward stack
                    _forwardStack.Clear();

                    // and we say history changed
                    RaiseHistoryChanged();
                    break;

                case NavigateMode.Back:

                    // we push the current item into the forward stack
                    _forwardStack.Push(_currentPageStatelessInfo);

                    // we remove one item
                    _backStack.Pop();

                    // and we say history changed
                    RaiseHistoryChanged();
                    break;

                case NavigateMode.Forward:

                    // we push the current item into the back stack
                    _backStack.Push(_currentPageStatelessInfo);

                    // and we remove one item
                    _forwardStack.Pop();

                    // and we say history changed and return
                    RaiseHistoryChanged();
                    break;

                case NavigateMode.Refresh:

                    // we don't need to do anything here, as the history remains the same
                    break;
            }

            // we add to the journal the current page info - note we don't save the state when refreshing
            if (base.CurrentNavigationRequest != null && !_isRestoring)
            {
                if (_states.ContainsKey(base.CurrentNavigationRequest.RequestUrl))
                {
                    _states[base.CurrentNavigationRequest.RequestUrl] = GetCurrentPageContentState();
                }
                else
                {
                    _states.Add(base.CurrentNavigationRequest.RequestUrl, GetCurrentPageContentState());
                    base.NotifyPropertyChanged(() => CanPurgeJournal);
                }
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

            // if the content supports state, and we are navigating back/forward
            if (CurrentNavigationStateSupporter != null)
            {
                // restore state, if we have a state
                if (_states.ContainsKey(response.Request.RequestUrl))
                {
                    CurrentNavigationStateSupporter.RestoreState(_states[response.Request.RequestUrl].State);  // note_ with stored state
                }
            }

            // if the content supports view state, and we are navigating back/forward
            if (CurrentNavigationViewStateSupporter != null)
            {
                if (_states.ContainsKey(response.Request.RequestUrl))
                {
                    CurrentNavigationViewStateSupporter.RestoreState(_states[response.Request.RequestUrl].VisualState);  // note_ with stored state
                }
            }

            // we save the url and the content
            CurrentNavigationRequest = response.Request;
            SetValue(ContentProperty, response.Content);

            // and raise the event navigation completed
            OnNavigationCompleted(new NavigatedEventArgs(this, response.Request));

            // and once we have set the transition state, then we call the view's initialize
            if (CurrentNavigationViewLifecycleSupporter != null) CurrentNavigationViewLifecycleSupporter.Initialize(response);
        }

        protected override ParametersCollection OnSaveState()
        {
            var _parameters = base.OnSaveState();
            if (_parameters == null) _parameters = new ParametersCollection();

            // we add to the journal the current page info
            if (base.CurrentNavigationRequest != null)
            {
                if (_states.ContainsKey(base.CurrentNavigationRequest.RequestUrl))
                {
                    _states[base.CurrentNavigationRequest.RequestUrl] = GetCurrentPageContentState();
                }
                else
                {
                    _states.Add(base.CurrentNavigationRequest.RequestUrl, GetCurrentPageContentState());
                    base.NotifyPropertyChanged(() => CanPurgeJournal);
                }
            }

            // note we store an array, and not a stack - as we shouldn't store a reference
            _parameters.Add(BACK_STACK_KEY, _backStack.Reverse().ToArray());
            _parameters.Add(FORWARD_STACK_KEY, _forwardStack.Reverse().ToArray());
            _parameters.Add(STATES_DICTIONAY_KEY, (_states.Select((kv) => new Object[] { kv.Key, kv.Value })).ToArray());

            return _parameters;
        }

        protected override void OnRestoreState(ParametersCollection state)
        {
            _backStack = new Stack<PageContentState>(state.GetValueOrDefault(BACK_STACK_KEY, Enumerable.Empty<PageContentState>()));
            _forwardStack = new Stack<PageContentState>(state.GetValueOrDefault(FORWARD_STACK_KEY, Enumerable.Empty<PageContentState>()));
            _states = new Dictionary<string, PageContentState>();

            var _keyValueStates = state.GetValueOrDefault(STATES_DICTIONAY_KEY, default(Object[][]));
            foreach (var _keyValue in _keyValueStates)
            {
                var _innerKeyValues = (Object[])_keyValue;      // this is crazy!
                _states.Add(Convert.ToString(_innerKeyValues[0]), (PageContentState)_innerKeyValues[1]);
            }

            _isRestoring = true;
            base.OnRestoreState(state);
            RaiseHistoryChanged();

            _isRestoring = false;
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
                while (this._backStack.Count > 0 && !toAnyOfUrls.Contains(_backPageInfo.Url))
                {
                    this._backStack.Pop();
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
            if (!CanNavigateBack && !CanNavigateForward && _states.Count == 0) return;
            _states.Clear();

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
            // we check the page has a url
            if (string.IsNullOrEmpty(this.Url)) return null;

            // we also add
            return new PageContentState(this.Url, CurrentNavigationRequest != null ? CurrentNavigationRequest.SiteArea : null, this.Title,
                (CurrentNavigationStateSupporter == null ? null : CurrentNavigationStateSupporter.SaveState()),
                (CurrentNavigationViewStateSupporter == null) ? null : CurrentNavigationViewStateSupporter.SaveState());
        }

        private PageContentState GetCurrentPageStatelessContentState()
        {
            // we check the page has a url
            if (string.IsNullOrEmpty(this.Url)) return null;

            // we also add
            return new PageContentState(this.Url, CurrentNavigationRequest != null ? CurrentNavigationRequest.SiteArea : null, this.Title, null);
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
