using nRoute.Internal;
using System;
using System.IO;

namespace nRoute.Navigation.Mapping
{
    public class NavigationResourceMeta
         : NavigationMetaBase
    {
        private readonly string _resourcePath;
        private readonly string _resourceAssembyName;
        private readonly Func<Stream, object> _resourceLoader;
        private readonly Func<object, ISupportNavigationState> _navigationStateManager;

        public NavigationResourceMeta(string url, string resourcePath, string resourceAssembyName,
            Func<Stream, Object> resourceLoader, Func<Object, ISupportNavigationState> navigationStateManager)
         : base(url)
        {
            Guard.ArgumentNotNullOrWhiteSpace(resourcePath, "resourcePath");
            Guard.ArgumentNotNull(resourceLoader, "resourceLoader");

            _resourcePath = resourcePath;
            _resourceAssembyName = resourceAssembyName;
            _resourceLoader = resourceLoader;
            _navigationStateManager = navigationStateManager;
        }

        #region Properties

        public string ResourcePath
        {
            get { return _resourcePath; }
        }

        public string ResourceAssembyName
        {
            get { return _resourceAssembyName; }
        }

        public Func<Stream, object> ResourceLoader
        {
            get { return _resourceLoader; }
        }

        public Func<object, ISupportNavigationState> NavigationStateManager
        {
            get { return _navigationStateManager; }
        }

        #endregion

    }
}