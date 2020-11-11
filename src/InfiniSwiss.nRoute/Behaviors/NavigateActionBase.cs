using nRoute.Components;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace nRoute.Behaviors
{
    [ContentProperty("Parameters")]
    public abstract class NavigateActionBase
        : NavigationHandlerActionBase
    {

        #region Constants and Variables

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(NavigateActionBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.Register("Parameters",
            typeof(DependencyParameterCollection), typeof(NavigateActionBase), null);

        public static readonly DependencyProperty SiteAreaProperty =
            DependencyProperty.Register("SiteArea", typeof(string), typeof(NavigateActionBase),
            new PropertyMetadata(null));

        #endregion

        #region Properties

        [Category("Common Properties")]
        public string Url
        {
            get { return Convert.ToString(GetValue(UrlProperty)); }
            set { SetValue(UrlProperty, value); }
        }

        [Category("Common Properties")]
        public DependencyParameterCollection Parameters
        {
            get
            {
                // lazy load, not the best idea but with DP's - but this way we can avoid unnecessary creation of parameters
                if (GetValue(ParametersProperty) == null)
                {
                    SetValue(ParametersProperty, new DependencyParameterCollection());
                }
                return (DependencyParameterCollection)GetValue(ParametersProperty);
            }
            set { SetValue(ParametersProperty, value); }
        }

        //#endif

        [Category("Common Properties")]
        public string SiteArea
        {
            get { return Convert.ToString(GetValue(SiteAreaProperty)); }
            set { SetValue(SiteAreaProperty, value); }
        }

        #endregion

    }
}
