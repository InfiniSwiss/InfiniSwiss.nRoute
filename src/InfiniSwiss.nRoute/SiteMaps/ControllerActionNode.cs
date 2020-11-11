using nRoute.Components;
using nRoute.Controllers;
using nRoute.Internal;
using nRoute.Navigation;
using System;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace nRoute.SiteMaps
{
    [Serializable]
    [XmlRoot(Namespace = SiteMaps.XMLNAMESPACE)]
    [ContentProperty("ChildNodes")]
    public class ControllerActionNode
         : SiteMapNode
    {
        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url",
            typeof(string), typeof(ControllerActionNode), new PropertyMetadata(null));

        public static readonly DependencyProperty UrlParametersProperty = DependencyProperty.Register("UrlParameters",
            typeof(DependencyParameterCollection), typeof(ControllerActionNode), new PropertyMetadata(null));

        public static readonly DependencyProperty HandlerNameProperty = DependencyProperty.Register("HandlerName",
            typeof(string), typeof(ControllerActionNode), new PropertyMetadata(null));

        #region Properties

        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE, IsNullable = false)]
        public string Url
        {
            get { return Convert.ToString(GetValue(UrlProperty)); }
            set { SetValue(UrlProperty, value); }
        }

        [XmlArray(Namespace = SiteMaps.XMLNAMESPACE)]
        public DependencyParameterCollection UrlParameters
        {
            get { return (DependencyParameterCollection)GetValue(UrlParametersProperty); }
            set { SetValue(UrlParametersProperty, value); }
        }

        [XmlElement(Namespace = SiteMaps.XMLNAMESPACE)]
        public string HandlerName
        {
            get { return Convert.ToString(GetValue(HandlerNameProperty)); }
            set { SetValue(HandlerNameProperty, value); }
        }

        #endregion

        #region Overrides

        public override void Execute()
        {
            Guard.ArgumentNotNullOrWhiteSpace(Url, "Url");
            if (!this.IsEnabled) return;

            ControllerService.Execute(
                new ControllerActionRequest(this.Url, this.UrlParameters, this.SiteArea, string.IsNullOrEmpty(HandlerName) ?
                NavigationService.DefaultNavigationHandler : NavigationService.GetNavigationHandler(HandlerName)));
        }


        #endregion

    }
}
