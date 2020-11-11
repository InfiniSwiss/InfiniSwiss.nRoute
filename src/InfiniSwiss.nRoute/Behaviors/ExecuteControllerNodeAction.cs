using Microsoft.Xaml.Behaviors;
using nRoute.Components.Routing;
using nRoute.Controllers;
using nRoute.SiteMaps;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class ExecuteControllerNodeAction
        : NavigationHandlerActionBase
    {
        private const string NODE_NOT_FOUND = "Could not resolve ControllerActionNode to execute Controller Action (ExecuteControllerNodeAction)";
        private const string ACTION_COULDNOT_EXECUTE = "Could not execute controller action, with url '{0}'";

        public static readonly DependencyProperty ControllerActionNodeKeyProperty =
           DependencyProperty.Register("ControllerActionNodeKey", typeof(string), typeof(ExecuteControllerNodeAction),
           new PropertyMetadata(null));

        public static readonly DependencyProperty ControllerActionNodeProperty =
           DependencyProperty.Register("ControllerActionNode", typeof(ControllerActionNode), typeof(ExecuteControllerNodeAction),
           new PropertyMetadata(null));

        #region Properties

        [Category("Common Properties")]
        public string ControllerActionNodeKey
        {
            get { return Convert.ToString(GetValue(ControllerActionNodeKeyProperty)); }
            set { SetValue(ControllerActionNodeKeyProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public ControllerActionNode ControllerActionNode
        {
            get { return (ControllerActionNode)GetValue(ControllerActionNodeProperty); }
            set { SetValue(ControllerActionNodeProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null) return;

            var _actionNode = this.ControllerActionNode;
            if (_actionNode == null && !string.IsNullOrEmpty(this.ControllerActionNodeKey))
            {
                if (!SiteMapService.IsSiteMapLoaded)
                {
                    SiteMapService.LoadSiteMap((s) => this.Invoke(null));
                    return;
                }
                _actionNode = SiteMapService.ResolveSiteMapNode(ControllerActionNodeKey) as ControllerActionNode;
            }

            if (_actionNode == null) throw new ControllerActionException(NODE_NOT_FOUND, ResponseStatus.HandlerNotFound);

            var _request = new ControllerActionRequest(_actionNode.Url, _actionNode.UrlParameters, _actionNode.SiteArea, base.ResolveHandler(), this.AssociatedObject);
            ControllerService.Execute(_request, (s) =>
            {
                if (s != ResponseStatus.Success && s != ResponseStatus.Cancelled)
                {
                    throw new ControllerActionException(string.Format(ACTION_COULDNOT_EXECUTE, _actionNode.Url), s, _request);
                }
            });
        }

        #endregion

    }
}
