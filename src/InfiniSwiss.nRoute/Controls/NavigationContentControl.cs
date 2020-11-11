using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Internal;
using nRoute.Navigation;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace nRoute.Controls
{
    [ContentProperty]
    public partial class NavigationContentControl
        : ContentControl, INotifyPropertyChanged
    {
        private const string NAVIGATION_FAILED_TITLE = "Navigation Problem";
        private const string NAVIGATION_FAILED_INFO = "Could not navigate to URL '{0}'.";
        private const string DEFAULT_ERRORPAGE_URL = "About:Error";

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NavigationContentControl), null);

        public static readonly DependencyProperty UrlProperty =
           DependencyProperty.Register("Url", typeof(string), typeof(NavigationContentControl),
           new PropertyMetadata(null, OnUrlPropertyChanged));

        public static readonly DependencyProperty ErrorUrlProperty =
           DependencyProperty.Register("ErrorUrl", typeof(string), typeof(NavigationContentControl),
           new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NaivationStateProperty =
            DependencyProperty.Register("NavigationState", typeof(NavigationState), typeof(NavigationContentControl),
            new PropertyMetadata(NavigationState.UnNavigated));

        private NavigationRequest _currentNavigationRequest;
        private NavigationRequest _activeNavigationRequest;

        public NavigationContentControl()
        {
            InitializeComponent();
        }

        protected virtual void InitializeComponent()
        {
            // we set the default style
            this.DefaultStyleKey = typeof(NavigationContentControl);
        }

        #region Properties

        [Category("Common Properties")]
        public virtual string Url
        {
            get { return Convert.ToString(this.GetValue(UrlProperty)); }
            set { SetValue(UrlProperty, value); }
        }

        [Category("Common Properties")]
        public virtual string ErrorUrl
        {
            get { return Convert.ToString(this.GetValue(ErrorUrlProperty)); }
            set { SetValue(ErrorUrlProperty, value); }
        }

        [Category("Common Properties")]
        public virtual string Title
        {
            get { return Convert.ToString(this.GetValue(TitleProperty)); }
            set { SetValue(TitleProperty, value); }
        }

        [Category("Common Properties")]
        public NavigationState NavigationState
        {
            get { return (NavigationState)this.GetValue(NaivationStateProperty); }
        }

        #endregion

        #region Navigation Methods

        public virtual void Navigate(string url)
        {
            Navigate(url, null);
        }

        public virtual void Navigate(string url, ParametersCollection requestParameters)
        {
            OnNavigate(new NavigationRequest(url, requestParameters, NavigateMode.New));
        }

        public virtual void Navigate(NavigationRequest request)
        {
            OnNavigate(request);
        }

        #endregion

        #region Internal

        protected ISupportNavigationLifecycle CurrentNavigationLifecycleSupporter { get; set; }

        protected ISupportNavigationViewLifecycle CurrentNavigationViewLifecycleSupporter { get; set; }

        protected ISupportNavigationState CurrentNavigationStateSupporter { get; set; }

        protected ISupportNavigationViewState CurrentNavigationViewStateSupporter { get; set; }

        protected NavigationRequest ActiveNavigationRequest
        {
            get { return _activeNavigationRequest; }
            set { _activeNavigationRequest = value; }
        }

        protected NavigationRequest CurrentNavigationRequest
        {
            get { return _currentNavigationRequest; }
            set
            {
                Guard.ArgumentNotNull(value, "LastRequest");
                _currentNavigationRequest = value;
                if (!string.Equals(this.Url, _currentNavigationRequest.RequestUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.SetValue(UrlProperty, _currentNavigationRequest.RequestUrl);
                }
                NotifyPropertyChanged(() => CanRefresh);
            }
        }

        protected virtual void TransitionToNavigationState(NavigationState navigationState)
        {
            this.SetValue(NaivationStateProperty, navigationState);
        }

        protected virtual void ShowFailedNavigationState(NavigationRequest request, ResponseStatus status, Exception error)
        {
            //if (request == null || string.IsNullOrEmpty(request.RequestUrl)) return;       // this is also for Blend 
            //if (Debugger.IsAttached) return;
            if (IsInDesignMode()) return;

            var _errorUrl = !string.IsNullOrEmpty(ErrorUrl) ? ErrorUrl : DEFAULT_ERRORPAGE_URL;

            // note_, we resolve
            Navigation.NavigationService.Resolve(new NavigationRequest(_errorUrl), async (r) =>
            {
                // set the stuff
                var _supporter = Navigation.NavigationService.GetSupporter<ISupportNavigationFailure>(r.Content);
                var _currentContent = Navigation.NavigationService.GetSupporter<ISupportNavigationFailure>(this.Content);
                if (_supporter != null)
                {
                    _supporter.Request = request;
                    _supporter.RefererRequest = _currentContent != null ? _currentContent.RefererRequest
                        ?? CurrentNavigationRequest : CurrentNavigationRequest;
                    _supporter.ResponseStatus = status;
                    _supporter.Error = error;
                }

                // changed this to process response because 
                //this.SetValue(ContentProperty, r.Content);
                //var _currentNavigationRequest = CurrentNavigationRequest;
                await this.ProcessResponseImplAsync(new NavigationResponse(ActiveNavigationRequest, ResponseStatus.Success, r.Content, null, error));
                //CurrentNavigationRequest = _currentNavigationRequest;

                // set the title
                Title = NAVIGATION_FAILED_TITLE;
            });
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            Guard.ArgumentNotNull(propertySelector, "propertySelector");
            PropertyChanged.Notify<T>(propertySelector);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Helpers

        protected bool IsInDesignMode()
        {
            if ((Application.Current != null) && (Application.Current.GetType() != typeof(Application)))
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
            return true;
        }

        #endregion

        #region Static Handler

        private static void OnUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigationContentControl)d).OnUrlChanged(Convert.ToString(e.OldValue), Convert.ToString(e.NewValue));
        }

        #endregion

    }
}