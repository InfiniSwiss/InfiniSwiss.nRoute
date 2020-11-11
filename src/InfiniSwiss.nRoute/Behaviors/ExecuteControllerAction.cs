using nRoute.Components;
using nRoute.Components.Routing;
using nRoute.Controllers;
using System.Windows.Markup;

namespace nRoute.Behaviors
{
    [ContentProperty("Parameters")]
    public class ExecuteControllerAction
         : NavigateActionBase
    {
        private const string ACTIONURL_NOT_VALID = "Cannot execute controller action without a valid url";
        private const string ACTION_COULDNOT_EXECUTE = "Could not execute controller action, with url '{0}'";

        public ExecuteControllerAction()
            : base() { }

        #region Trigger Related

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null) return;

            if (string.IsNullOrEmpty(this.Url))
            {
                throw new ControllerActionException(ACTIONURL_NOT_VALID, ResponseStatus.UrlInvalid);
            }

            var _url = Url;
            var _parameters = (DependencyParameterCollection)GetValue(ParametersProperty);
            var _handler = ResolveHandler();

            var _request = new ControllerActionRequest(_url, _parameters, this.SiteArea, _handler, this.AssociatedObject);
            ControllerService.Execute(_request, (s) =>
            {
                if (s != ResponseStatus.Success && s != ResponseStatus.Cancelled)
                {
                    throw new ControllerActionException(string.Format(ACTION_COULDNOT_EXECUTE, _url), s, _request);
                }
            });
        }

        #endregion

    }
}
