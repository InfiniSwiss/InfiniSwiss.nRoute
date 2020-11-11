using nRoute.Components;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace nRoute.SiteMaps
{
    /// <summary>
    /// Xaml declared <see cref="ISiteMapProvider">Site Map Provider</see>.
    /// </summary>
    [ContentProperty("SiteMap")]
    public class XamlSiteMapProvider
       : DependencyObject, ISiteMapProvider
    {
        private const string SITEMAP_SET_ONCE = "Site Map can only be set once.";

        //private SiteMap _siteMap;

        public static readonly DependencyProperty SiteMapProperty =
            DependencyProperty.Register("SiteMap", typeof(SiteMap), typeof(XamlSiteMapProvider),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSiteMapChanged)));

        /// <summary>
        /// <see cref="SiteMap">Site Map</see> declared in xaml.
        /// </summary>
        [Category("Common Properties")]
        public SiteMap SiteMap
        {
            get { return (SiteMap)GetValue(SiteMapProperty); }
            set { SetValue(SiteMapProperty, value); }
        }

        //public SiteMap SiteMap
        //{
        //    get { return _siteMap; }
        //    set
        //    {
        //        if (_siteMap != null) throw new InvalidOperationException(SITEMAP_SET_ONCE);
        //        _siteMap = value;
        //    }
        //}

        #region ISiteMapProvider Members

        public IObservable<SiteMap> ResolveSiteMap()
        {
            var _observable = new LazyRelayObservable<SiteMap>((o) =>
            {
                o.OnNext(this.SiteMap);
                o.OnCompleted();
            });
            return _observable;
        }

        #endregion

        #region Handlers

        private static void OnSiteMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null) throw new InvalidOperationException(SITEMAP_SET_ONCE);
        }

        #endregion

    }
}
