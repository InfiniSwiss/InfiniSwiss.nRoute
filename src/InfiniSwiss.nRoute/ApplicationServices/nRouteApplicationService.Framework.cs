using nRoute.SiteMaps;
using System;
using System.Windows.Markup;

namespace nRoute.ApplicationServices
{
    [ContentProperty("SiteMapProvider")]
    public partial class nRouteApplicationService
    {
        private const string SITEMAP_ONLYSETONCE = "Application-wide SiteMap can only be set once.";

        private ISiteMapProvider _siteMapProvider;

        public ISiteMapProvider SiteMapProvider
        {
            get { return _siteMapProvider; }
            set
            {
                if (_siteMapProvider != null) throw new InvalidOperationException(SITEMAP_ONLYSETONCE);
                lock (_lock)
                {
                    if (_siteMapProvider == null)
                    {
                        _siteMapProvider = value;
                    }
                }
            }
        }

        public string ApplicationExceptionUrl { get; set; }
    }
}
