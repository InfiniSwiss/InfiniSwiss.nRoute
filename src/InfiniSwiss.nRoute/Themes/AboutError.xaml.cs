using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace nRoute.Themes
{
    public partial class AboutError
        : UserControl,
        ISupportNavigationFailure, INotifyPropertyChanged
    {

        #region Declarations

        private readonly static Brush HIGHLIGHT_BRUSH = new SolidColorBrush(Color.FromArgb(255, 38, 75, 108));
        private readonly static Brush NORMAL_BRUSH = new SolidColorBrush(Color.FromArgb(255, 115, 169, 216));

        private const string URL_NOTFOUND_TITLE = "Requested Url Not Found";
        private const string URL_NOTFOUND_SUBTITLE = "Could not find Url: '{0}'";
        private const string URL_INVALID_TITLE = "Requested Url is Invalid";
        private const string URL_INVALID_SUBTITLE = "Invalid navigation Url: '{0}'";
        private const string HANDLER_NOTFOUND_TITLE = "Navigation Handler Not Found";
        private const string HANDLER_NOTFOUND_SUBTITLE = "No handler found for Url: '{0}'";
        private const string TIMEOUT_TITLE = "Navigation Request Timed Out";
        private const string TIMEOUT_SUBTITLE = "A time-out requesting Url: '{0}'";
        private const string EXCEPTION_TITLE = "An Error Occurred";
        private const string EXCEPTION_SUBTITLE = "Error requesting for Url: '{0}'";
        private const string NAVIGATE_RETURN_FORMAT = "Return to '{0}'";
        private const string NAVIGATE_RETRY_FORMAT = "Retry Navigating to '{0}'";

        private NavigationRequest _request;
        private NavigationRequest _refererRequest;
        private ResponseStatus _responseStatus;
        private Exception _error;
        private INavigationHandler _handler;

        #endregion

        public AboutError()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.DataContext = this;

            this.SetRelayConverter<ResponseStatus, string>("TitleConverterRelay",
                (r) =>
                {
                    switch (r)
                    {
                        case ResponseStatus.UrlNotFound:
                            return URL_NOTFOUND_TITLE;
                        case ResponseStatus.UrlInvalid:
                            return URL_INVALID_TITLE;
                        case ResponseStatus.HandlerNotFound:
                            return HANDLER_NOTFOUND_TITLE;
                        case ResponseStatus.Timeout:
                            return TIMEOUT_TITLE;
                        case ResponseStatus.Exception:
                            return EXCEPTION_TITLE;
                        case ResponseStatus.Success:
                        case ResponseStatus.Cancelled:
                            throw new NotSupportedException();
                        default:
                            throw new ArgumentOutOfRangeException("r");
                    }
                });

            this.SetRelayConverter<ResponseStatus, string>("SubTitleConverterRelay",
                (r) =>
                {
                    switch (r)
                    {
                        case ResponseStatus.UrlNotFound:
                            return string.Format(URL_NOTFOUND_SUBTITLE, Request.RequestUrl);
                        case ResponseStatus.UrlInvalid:
                            return string.Format(URL_INVALID_SUBTITLE, Request.RequestUrl);
                        case ResponseStatus.HandlerNotFound:
                            return string.Format(HANDLER_NOTFOUND_SUBTITLE, Request.RequestUrl);
                        case ResponseStatus.Timeout:
                            return string.Format(TIMEOUT_SUBTITLE, Request.RequestUrl);
                        case ResponseStatus.Exception:
                            return string.Format(EXCEPTION_SUBTITLE, Request.RequestUrl);
                        case ResponseStatus.Success:
                        case ResponseStatus.Cancelled:
                            throw new NotSupportedException();
                        default:
                            throw new ArgumentOutOfRangeException("r");
                    }
                });

            this.SetRelayConverter<NavigationRequest, string>("ReturnUrlConverterRelay",
                (n) => n == null ? string.Empty : string.Format(NAVIGATE_RETURN_FORMAT, n.RequestUrl));

            this.SetRelayConverter<NavigationRequest, string>("RetryUrlConverterRelay",
                (n) => n == null ? string.Empty : string.Format(NAVIGATE_RETRY_FORMAT, n.RequestUrl));

        }

        #region ISupportNavigationFailure Members

        public NavigationRequest Request
        {
            get { return _request; }
            set
            {
                if (_request != value)
                {
                    _request = value;
                    PropertyChanged.Notify(() => Request);
                }
            }
        }

        public NavigationRequest RefererRequest
        {
            get { return _refererRequest; }
            set
            {
                if (_refererRequest != value)
                {
                    _refererRequest = value;
                    PropertyChanged.Notify(() => RefererRequest);
                }
            }
        }

        public ResponseStatus ResponseStatus
        {
            get { return _responseStatus; }
            set
            {
                if (_responseStatus != value)
                {
                    _responseStatus = value;
                    PropertyChanged.Notify(() => ResponseStatus);
                }
            }
        }

        public Exception Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    PropertyChanged.Notify(() => Error);
                }
            }
        }

        #endregion

        public ISupportDirectionalNavigation DirectionalNavigationHandler
        {
            get
            {
                return (NavigationHandler as ISupportDirectionalNavigation);
            }
        }

        private INavigationHandler NavigationHandler
        {
            get
            {
                if (_handler == null)
                {
                    _handler = nRoute.Navigation.NavigationService.ResolveHandlerInVisualTree(this);
                }
                return _handler;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Handlers

        private void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = NORMAL_BRUSH;
        }

        private void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = HIGHLIGHT_BRUSH;
        }

        private void Link_NavigateReturn(object sender, MouseEventArgs e)
        {
            if (RefererRequest == null) return;

            var _backRequest = new NavigationRequest(RefererRequest.RequestUrl, RefererRequest.RequestParameters,
                RefererRequest.SiteArea, NavigateMode.New);
            nRoute.Navigation.NavigationService.Navigate(_backRequest, NavigationHandler);
        }

        private void Link_NavigateRetry(object sender, MouseEventArgs e)
        {
            if (Request == null) return;

            var _retryRequest = new NavigationRequest(Request.RequestUrl, Request.RequestParameters, Request.SiteArea, NavigateMode.Refresh);
            nRoute.Navigation.NavigationService.Navigate(_retryRequest, NavigationHandler);
        }

        private void Link_NavigateBack(object sender, MouseEventArgs e)
        {
            if (DirectionalNavigationHandler != null && DirectionalNavigationHandler.CanNavigateBack)
            {
                DirectionalNavigationHandler.NavigateBack();
            }
        }

        #endregion

    }
}
