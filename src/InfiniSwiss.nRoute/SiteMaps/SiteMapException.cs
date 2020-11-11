using nRoute.Components.Routing;
using System;

namespace nRoute.SiteMaps
{
    [Serializable]
    public class SiteMapException
        : Exception
    {
        private readonly IUrlRequest _request;
        private readonly string _siteMapNode;

        public SiteMapException(string message)
            : this(message, null, null, null) { }

        public SiteMapException(string message, string siteMapNode)
            : this(message, siteMapNode, null, null) { }

        public SiteMapException(string message, string siteMapNode, IUrlRequest request)
            : this(message, siteMapNode, request, null) { }

        public SiteMapException(string message, string siteMapNode, IUrlRequest request, Exception innerException)
            : base(message, innerException)
        {
            _siteMapNode = siteMapNode;
            _request = request;
        }

        #region Properties

        public string SiteMapNode
        {
            get { return _siteMapNode; }
        }

        public IUrlRequest Request
        {
            get { return _request; }
        }

        #endregion

    }
}
