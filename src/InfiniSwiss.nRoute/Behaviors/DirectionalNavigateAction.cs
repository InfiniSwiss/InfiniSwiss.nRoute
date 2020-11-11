using nRoute.Components.Routing;
using nRoute.Navigation;
using nRoute.Navigation.Containers;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class DirectionalNavigateAction
        : NavigationHandlerActionBase
    {
        private const string HANDLER_NOTFOUND = "Could not resolve handler to navigate directionally (DirectionalNavigateAction)";
        private const string HANDLER_MUSTSUPPORT_DIRECTIONALNAVIGATION =
            "Handler does not support directional navigation (ISupportDirectionalNavigation)";

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(NavigationDirection), typeof(DirectionalNavigateAction),
            new PropertyMetadata(NavigationDirection.Back));

        #region Properties

        [Category("Common Properties")]
        public NavigationDirection Direction
        {
            get { return (NavigationDirection)(GetValue(DirectionProperty)); }
            set { SetValue(DirectionProperty, value); }
        }

        #endregion

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

            // get the directional handler
            var _directionHandler = _handler as ISupportDirectionalNavigation;
            if (_directionHandler == null)
            {
                throw new NavigationException(HANDLER_MUSTSUPPORT_DIRECTIONALNAVIGATION, ResponseStatus.HandlerNotFound);
            }

            // we navigate if can do..
            if (this.Direction == NavigationDirection.Back)
            {
                if (_directionHandler.CanNavigateBack)
                {
                    _directionHandler.NavigateBack();
                }
            }
            else
            {
                if (_directionHandler.CanNavigateForward)
                {
                    _directionHandler.NavigateForward();
                }
            }
        }

        #endregion

    }
}
