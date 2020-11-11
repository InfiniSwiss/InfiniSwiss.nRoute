using nRoute.Components.Routing;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
namespace nRoute.Behaviors
{
    public class RefreshNavigationAction
        : NavigationHandlerActionBase
    {
        private const string HANDLER_NOTFOUND = "Could not resolve handler for refreshing (RefreshNavigationAction)";
        private const string HANDLER_MUSTSUPPORT_REFRESHNAVIGATION = "Handler does not support refresh navigation (ISupportRefreshNavigation)";

        #region Overrides

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null) return;

            // get the handler, if not found then throw
            var _handler = base.ResolveHandler();
            if (_handler == null)
            {
                throw new NavigationException(HANDLER_NOTFOUND, ResponseStatus.HandlerNotFound);
            }

            // get the refresh handler
            var _refreshHandler = _handler as ISupportRefreshNavigation;
            if (_refreshHandler == null)
            {
                throw new NavigationException(HANDLER_MUSTSUPPORT_REFRESHNAVIGATION, ResponseStatus.HandlerNotFound);
            }

            // and refresh
            if (_refreshHandler.CanRefresh)
            {
                _refreshHandler.Refresh();
            }
        }

        #endregion

    }
}
