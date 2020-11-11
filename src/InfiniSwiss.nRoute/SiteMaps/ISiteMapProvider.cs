using System;

namespace nRoute.SiteMaps
{
    /// <summary>
    /// Resolves a <see cref="SiteMap">Site Map</see>, asyncronously from any internal or external source.
    /// </summary>
    public interface ISiteMapProvider
    {
        /// <summary>
        /// Requests for a <see cref="SiteMap">Site Map</see> to be resolved.
        /// </summary>
        IObservable<SiteMap> ResolveSiteMap();
    }
}
