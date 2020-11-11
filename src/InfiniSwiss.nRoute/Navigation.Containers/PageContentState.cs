using nRoute.Components;
using System;

namespace nRoute.Navigation.Containers
{
    [Serializable]
    public class PageContentState
    {
        private string _url;
        private string _siteArea;
        private string _title;
        private ParametersCollection _state;
        private ParametersCollection _visualState;

        public PageContentState() { }

        public PageContentState(string url, string siteArea, string title, ParametersCollection state)
        {
            _url = url;
            _siteArea = siteArea;
            _title = title;
            _state = state;
        }

        public PageContentState(string url, string siteArea, string title, ParametersCollection state, ParametersCollection visualState)
            : this(url, siteArea, title, state)
        {
            _visualState = visualState;
        }

        #region Properties

        public virtual string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public virtual string SiteArea
        {
            get { return _siteArea; }
            set { _siteArea = value; }
        }

        public virtual string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public virtual ParametersCollection State
        {
            get { return _state; }
            set { _state = value; }
        }

        public virtual ParametersCollection VisualState
        {
            get { return _visualState; }
            set { _visualState = value; }
        }

        #endregion

        #region Operators

        public static implicit operator NavigationRequest(PageContentState pageContentState)
        {
            if (pageContentState == null) return null;

            return new NavigationRequest(pageContentState.Url, pageContentState.State, pageContentState.SiteArea, NavigateMode.Refresh)
            {
                ServiceState = pageContentState.VisualState
            };
        }

        public static implicit operator PageContentState(NavigationRequest navigationRequest)
        {
            if (navigationRequest == null) return null;

            return new PageContentState(navigationRequest.RequestUrl, navigationRequest.SiteArea, null, navigationRequest.RequestParameters,
                navigationRequest.ServiceState as ParametersCollection);
        }

        #endregion

    }
}
