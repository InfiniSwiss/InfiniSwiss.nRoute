using Microsoft.Xaml.Behaviors;
using nRoute.Components.Routing;
using nRoute.Navigation;
using nRoute.SiteMaps;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NavigateNodeAction
        : NavigationHandlerActionBase
    {
        private const string HANDLER_NOT_RESOLVE = "Could not resolve INavigationHandler to navigate SiteMap (NavigateSiteMapNodeAction)";
        private const string NODE_NOT_FOUND = "Could not resolve NavigationNode for navigate SiteMap (NavigateSiteMapNodeAction)";

        public static readonly DependencyProperty NavigationNodeKeyProperty =
           DependencyProperty.Register("NavigationNodeKey", typeof(string), typeof(NavigateNodeAction),
           new PropertyMetadata(null));

        public static readonly DependencyProperty NavigationNodeProperty =
           DependencyProperty.Register("NavigationNode", typeof(NavigationNode), typeof(NavigateNodeAction),
           new PropertyMetadata(null));

        #region Properties

        [Category("Common Properties")]
        public string NavigationNodeKey
        {
            get { return Convert.ToString(GetValue(NavigationNodeKeyProperty)); }
            set { SetValue(NavigationNodeKeyProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public NavigationNode NavigationNode
        {
            get { return (NavigationNode)GetValue(NavigationNodeProperty); }
            set { SetValue(NavigationNodeProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null) return;

            // resolve the handler
            var _handler = base.ResolveHandler();
            if (_handler == null) throw new NavigationException(HANDLER_NOT_RESOLVE, ResponseStatus.HandlerNotFound);

            // resolve the navigation node
            var _navigationNode = this.NavigationNode;
            if (_navigationNode == null && !string.IsNullOrEmpty(this.NavigationNodeKey))
            {
                if (!SiteMapService.IsSiteMapLoaded)
                {
                    SiteMapService.LoadSiteMap((s) => this.Invoke(null));
                    return;
                }
                _navigationNode = SiteMapService.ResolveSiteMapNode(NavigationNodeKey) as NavigationNode;
            }

            if (_navigationNode == null) throw new NavigationException(NODE_NOT_FOUND, ResponseStatus.UrlNotFound);

            // and navigate
            NavigationService.Navigate(new NavigationRequest(_navigationNode.Url, _navigationNode.UrlParameters,
                _navigationNode.SiteArea, NavigateMode.New), _handler);
        }

        #endregion

    }
}
