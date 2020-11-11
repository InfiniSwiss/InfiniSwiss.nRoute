using Microsoft.Xaml.Behaviors;
using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Controls;
using nRoute.Internal;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;

namespace nRoute.Behaviors
{
    public abstract partial class NavigationAdapterBehaviorBase<T>
        : Behavior<T>, INotifyPropertyChanged
        where
 T : UIElement
    {
        private const string NAVIGATIONURL_NOTNULLOREMPTY = "Request's navigation url cannot be null or empty.";
        private const string DEFAULT_ERRORPAGE_URL = "About:Error";
        private const string NAVIGATION_FAILED_TITLE = "Navigation Problem";
        private const string NAVIGATION_FAILED_INFO = "Could not navigate to URL '{0}'.";

        public static readonly DependencyProperty TitleProperty =
         DependencyProperty.Register("Title", typeof(string), typeof(NavigationAdapterBehaviorBase<T>), null);

        public static readonly DependencyProperty UrlProperty =
           DependencyProperty.Register("Url", typeof(string), typeof(NavigationAdapterBehaviorBase<T>),
           new PropertyMetadata(null, OnUrlPropertyChanged));

        public static readonly DependencyProperty ErrorUrlProperty =
           DependencyProperty.Register("ErrorUrl", typeof(string), typeof(NavigationAdapterBehaviorBase<T>),
           new PropertyMetadata(null, OnUrlPropertyChanged));

        public static readonly DependencyProperty NavigationStateProperty =
            DependencyProperty.Register("NavigationState", typeof(NavigationState), typeof(NavigationAdapterBehaviorBase<T>),
            new PropertyMetadata(NavigationState.UnNavigated));

        private NavigationRequest _currentNavigationRequest;
        private NavigationRequest _activeNavigationRequest;

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();
            Navigation.Navigation.SetHandler(this.AssociatedObject, this);      // this makes it a navigation handler
        }

        #endregion

        #region INavigationContainer Members

        public event EventHandler<NavigatingCancelEventArgs> Navigating;

        public event EventHandler<NavigatedEventArgs> Navigated;

        public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        public abstract object Content { get; }

        [Category("Common Properties")]
        public string Url
        {
            get { return Convert.ToString(this.GetValue(UrlProperty)); }
            set { SetValue(UrlProperty, value); }
        }

        [Category("Common Properties")]
        public string Title
        {
            get { return Convert.ToString(this.GetValue(TitleProperty)); }
            set { SetValue(TitleProperty, value); }
        }

        public virtual void Navigate(NavigationRequest request)
        {
            OnNavigate(request);
        }

        #endregion

        #region Additional Properties

        [Category("Common Properties")]
        public virtual string ErrorUrl
        {
            get { return Convert.ToString(this.GetValue(ErrorUrlProperty)); }
            set { SetValue(ErrorUrlProperty, value); }
        }

        [Category("Common Properties")]
        public NavigationState NavigationState
        {
            get { return (NavigationState)GetValue(NavigationStateProperty); }
        }

        #endregion

        #region Additional Navigation Methods

        public virtual void Navigate(string url)
        {
            Navigate(url, null);
        }

        public virtual void Navigate(string url, ParametersCollection requestParameters)
        {
            OnNavigate(new NavigationRequest(url, requestParameters, NavigateMode.New));
        }

        #endregion

        #region Protected Functionality

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
                Guard.ArgumentNotNull(value, "CurrentRequest");
                _currentNavigationRequest = value;
                this.Url = _currentNavigationRequest.RequestUrl;
                NotifyPropertyChanged(() => CanRefresh);
            }
        }

        protected virtual void TransitionToNavigationState(NavigationState navigationState)
        {
            this.SetValue(NavigationStateProperty, navigationState);
        }

        protected virtual void ShowFailedNavigationState(NavigationRequest request, ResponseStatus status, Exception error)
        {
            //if (request == null || string.IsNullOrEmpty(request.RequestUrl)) return;       // this is also for Blend 
            //if (Debugger.IsAttached) return;
            if (IsInDesignMode()) return;

            var _errorUrl = !string.IsNullOrEmpty(ErrorUrl) ? ErrorUrl : DEFAULT_ERRORPAGE_URL;

            // note_, we resolve
            NavigationService.Resolve(new NavigationRequest(_errorUrl), async (r) =>
            {
                // set the stuff
                var _supporter = NavigationService.GetSupporter<ISupportNavigationFailure>(r.Content);
                var _currentContent = NavigationService.GetSupporter<ISupportNavigationFailure>(this.Content);
                if (_supporter != null)
                {
                    _supporter.Request = request;
                    _supporter.RefererRequest = _currentContent != null ? _currentContent.RefererRequest ?? CurrentNavigationRequest : CurrentNavigationRequest;
                    _supporter.ResponseStatus = status;
                    _supporter.Error = error;
                }

                // changed this to process response because 
                //this.SetValue(ContentProperty, r.Content);
                //var _currentNavigationRequest = CurrentNavigationRequest;
                await this.OnProcessResponseAsync(new NavigationResponse(ActiveNavigationRequest, ResponseStatus.Success, r.Content, null, error));
                //CurrentNavigationRequest = _currentNavigationRequest;

                // set the title
                Title = NAVIGATION_FAILED_TITLE;
            });
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected void NotifyPropertyChanged<P>(Expression<Func<P>> propertySelector)
        {
            Guard.ArgumentNotNull(propertySelector, "propertySelector");
            PropertyChanged.Notify<P>(propertySelector);
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
            ((NavigationAdapterBehaviorBase<T>)d).OnUrlChanged(Convert.ToString(e.OldValue), Convert.ToString(e.NewValue));
        }

        #endregion

    }
}
