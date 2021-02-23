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
    public partial class StatefulContainer
         : IStatefulContainer
    {
        private const string STATES_DICTIONAY_KEY = "_StatefulStates";

        private Dictionary<string, PageContentState> _states;

        #region Constructor

        public StatefulContainer()
        {
            _states = new Dictionary<string, PageContentState>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region IStatefulContainer Members

        public event EventHandler<NavigationContainerEventArgs> JournalPurged;

        public IEnumerable<PageContentState> NavigatedPagesJournal
        {
            get { return _states.Values; }
        }

        public bool HasState(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return _states.ContainsKey(url);
        }

        public void PurgeJournal()
        {
            OnPurgeJournals();
        }

        public bool CanPurgeJournal
        {
            get { return (_states != null && _states.Count > 0); }
        }

        #endregion

        #region Overrides

        protected async override Task ProcessResponseImplAsync(NavigationResponse response)
        {
            Guard.ArgumentNotNull(response, "response");

            // we won't handle it the response, if it is not the active request!
            if (!response.Request.Equals(base.ActiveNavigationRequest)) return;

            // we check if the navigation was not successful we let the base class handle it
            if (response.Status != ResponseStatus.Success)
            {
                await base.ProcessResponseImplAsync(response);
                return;
            }

            // note_ we don't send this request to the base class, hereon
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

            // we save our supporters and use them - note_ we need to save this to access later
            CurrentNavigationLifecycleSupporter = NavigationService.GetSupporter<ISupportNavigationLifecycle>(response.Content);
            CurrentNavigationViewLifecycleSupporter = NavigationService.GetSupporter<ISupportNavigationViewLifecycle>(response.Content);
            CurrentNavigationStateSupporter = NavigationService.GetSupporter<ISupportNavigationState>(response.Content);
            CurrentNavigationViewStateSupporter = NavigationService.GetSupporter<ISupportNavigationViewState>(response.Content);

            // we save the url and the content
            CurrentNavigationRequest = response.Request;
            SetValue(ContentProperty, response.Content);
            
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

            // we get the page state manager and restore the state
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


            // and raise the event navigation completed
            OnNavigationCompleted(new NavigatedEventArgs(this, response.Request));

            // and once we have set the transition state, then we call the view's initialize
            if (CurrentNavigationViewLifecycleSupporter != null) CurrentNavigationViewLifecycleSupporter.Initialize(response);
        }

        protected override ParametersCollection OnSaveState()
        {
            var _parameters = base.OnSaveState();

            if (_states.Count > 0)
            {
                if (_parameters == null) _parameters = new ParametersCollection();
                _parameters.Add(STATES_DICTIONAY_KEY, (_states.Select((kv) => new Object[] { kv.Key, kv.Value })).ToArray());
            }

            return _parameters;
        }

        protected override void OnRestoreState(ParametersCollection state)
        {
            _states = new Dictionary<string, PageContentState>();

            var _keyValueStates = state.GetValueOrDefault(STATES_DICTIONAY_KEY, default(Object[][]));
            foreach (var _keyValue in _keyValueStates)
            {
                var _innerKeyValues = (Object[])_keyValue;      // this is crazy!
                foreach (var _innerKeyValue in _innerKeyValues)
                {
                    var _object = (Object[])_innerKeyValue;     // this is crazy too!
                    _states.Add(Convert.ToString(_object[0]), (PageContentState)_object[1]);
                }
            }

            base.OnRestoreState(state);
        }

        #endregion

        #region Journals Related

        protected virtual void OnPurgeJournals()
        {
            if (_states.Count == 0) return;
            _states.Clear();

            OnJournalsPurged(new NavigationContainerEventArgs(this));
            base.NotifyPropertyChanged(() => CanPurgeJournal);
        }

        #endregion

        #region Events Related

        protected virtual void OnJournalsPurged(NavigationContainerEventArgs e)
        {
            if (JournalPurged != null) JournalPurged(this, e);
        }

        #endregion

        #region Helpers

        private PageContentState GetCurrentPageContentState()
        {
            // we check the page has a url
            if (base.CurrentNavigationRequest == null) return null;

            // we return the state
            return new PageContentState(this.Url, CurrentNavigationRequest != null ? CurrentNavigationRequest.SiteArea : null, this.Title,
                (CurrentNavigationStateSupporter == null ? null : CurrentNavigationStateSupporter.SaveState()),
                (CurrentNavigationViewStateSupporter == null) ? null : CurrentNavigationViewStateSupporter.SaveState());
        }

        #endregion

    }
}
