using nRoute.Internal;
using System;

namespace nRoute.Components.Mapping
{
    public sealed class LoadedResourceInfo
    {
        private readonly Uri _resourceUri;
        private readonly bool _isLoaded;
        private readonly Exception _error;

        internal LoadedResourceInfo(Uri resourceUri, bool isLoaded, Exception error)
        {
            Guard.ArgumentNotNull(resourceUri, "resourceUri");
            _resourceUri = resourceUri;
            _isLoaded = isLoaded;
            _error = error;
        }

        public Uri ResourceUri
        {
            get { return _resourceUri; }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }

        public Exception Error
        {
            get { return _error; }
        }
    }
}
