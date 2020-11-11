using nRoute.Internal;

namespace nRoute.Navigation.Containers
{
    public class NavigatedEventArgs
         : NavigationContainerEventArgs
    {
        private readonly NavigationRequest _request;

        public NavigatedEventArgs(INavigationContainer container, NavigationRequest request)
         : base(container)
        {
            Guard.ArgumentNotNull(request, "request");
            _request = request;
        }

        #region Properties

        public NavigationRequest Request
        {
            get
            {
                return _request;
            }
        }

        #endregion

    }
}
