using Microsoft.Xaml.Behaviors;
using nRoute.SiteMaps;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class ExecuteSiteMapNodeAction
        : TriggerAction<DependencyObject>
    {
        private const string SITEMAPNODE_NOTFOUND = "SiteMap Node with key '{0}' not found";

        public static readonly DependencyProperty SiteMapNodeProperty =
            DependencyProperty.Register("SiteMapNode", typeof(SiteMapNode), typeof(ExecuteSiteMapNodeAction),
            new PropertyMetadata(null));

        public static readonly DependencyProperty SiteMapNodeKeyProperty =
            DependencyProperty.Register("SiteMapNodeKey", typeof(string), typeof(ExecuteSiteMapNodeAction),
            new PropertyMetadata(null));

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public SiteMapNode SiteMapNode
        {
            get { return (SiteMapNode)GetValue(SiteMapNodeProperty); }
            set { SetValue(SiteMapNodeProperty, value); }
        }

        [Category("Common Properties")]
        public string SiteMapNodeKey
        {
            get { return Convert.ToString(GetValue(SiteMapNodeKeyProperty)); }
            set { SetValue(SiteMapNodeKeyProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void Invoke(object parameter)
        {
            if (SiteMapNode != null)
            {
                SiteMapNode.Execute();
            }
            else if (!string.IsNullOrEmpty(SiteMapNodeKey))
            {
                if (!SiteMapService.IsSiteMapLoaded)
                {
                    SiteMapService.LoadSiteMap((s) =>
                    {
                        this.Invoke(null);
                    });
                    return;
                }

                var _siteMapNode = SiteMapService.ResolveSiteMapNode(SiteMapNodeKey);
                if (_siteMapNode == null)
                {
                    throw new InvalidOperationException(string.Format(SITEMAPNODE_NOTFOUND, SiteMapNodeKey));
                }
                _siteMapNode.Execute();
            }
        }

        #endregion

    }
}
