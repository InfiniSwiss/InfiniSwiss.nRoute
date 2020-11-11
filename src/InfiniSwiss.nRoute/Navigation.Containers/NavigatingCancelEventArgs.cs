using nRoute.Internal;

namespace nRoute.Navigation.Containers
{
    public class NavigatingCancelEventArgs
         : NavigationContainerEventArgs
    {
        private readonly NavigationRequest _request;

        public NavigatingCancelEventArgs(INavigationContainer container, NavigationRequest request)
         : base(container)
        {
            Guard.ArgumentNotNull(request, "request");
            _request = request;
        }

        #region Properties

        public NavigationRequest Request
        {
            get { return _request; }
        }

        public bool Cancel { get; set; }

        #endregion

    }
}
