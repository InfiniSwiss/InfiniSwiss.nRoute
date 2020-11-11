using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Navigation;
using System.Windows.Markup;

namespace nRoute.Behaviors
{
    [ContentProperty("Parameters")]
    public class NavigateAction
         : NavigateActionBase
    {

        #region Constants and Variables

        private const string CANNOT_NAVIGATE_URL = "NavigateBehavior cannot navigate without a valid url";
        private const string CANNOT_NAVIGATE_TO_URL = "Cannot navigate to '{0}', no navigation handler resolver";

        #endregion

        #region Trigger Related

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null) return;
            if (string.IsNullOrEmpty(this.Url)) return;

            // get the navigation request
            var _url = Url;
            var _parameters = (DependencyParameterCollection)GetValue(ParametersProperty);
            var _request = new NavigationRequest(_url, _parameters, SiteArea, NavigateMode.New);

            // get the handler
            var _handler = ResolveHandler();
            if (_handler == null) throw new NavigationException(string.Format(CANNOT_NAVIGATE_TO_URL, this.Url),
                ResponseStatus.HandlerNotFound, _request);

            // navigate
            NavigationService.Navigate(_request, _handler);
        }

        #endregion

    }
}
