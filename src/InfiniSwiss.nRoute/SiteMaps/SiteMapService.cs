using nRoute.ApplicationServices;
using nRoute.Components;
using nRoute.Components.Mapping;
using nRoute.Components.Messaging;
using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.SiteMaps
{
    public static class SiteMapService
    {
        private static SiteMap _siteMap;
        private static readonly Object _lock = new Object();
        private static bool _isLoaded;

        #region Initialize Related

        internal static void Initialize()
        {
            var _nRouteService =
                ((nRoute.ApplicationServices.Application)System.Windows.Application.Current).
                    ApplicationLifetimeObjects.OfType<nRouteApplicationService>().FirstOrDefault();
            if (_nRouteService != null && _nRouteService.SiteMapProvider != null)
            {
                var _resolver = _nRouteService.SiteMapProvider.ResolveSiteMap();
                ObservableExtensions.Subscribe(_resolver, s =>
                {
                    lock (_lock)
                    {
                        _siteMap = s;
                        _isLoaded = true;
                    }
                    if (SiteMap.Areas != null)
                    {
                        TopologicallyOrderDependentSiteAreas(s.Areas);
                        InitializeOnLoadSiteAreas();
                    }
                },
                (ex) => { throw ex; },
                () => Channel.Publish(_siteMap));    // we tell the world that a site map has been published
            }
        }

        #endregion

        #region Areas Related

        public static bool IsSiteAreaInitialized(string key)
        {
            Guard.ArgumentNotNullOrEmpty(key, "key");

            lock (_lock)
            {
                return (SiteMap != null && SiteMap.Areas != null && SiteMap.Areas.Contains(key) &&
                        SiteMap.Areas[key].InternalIsInitialized);
            }
        }

        public static void InitializeSiteArea(string key)
        {
            InitializeSiteArea(key, null);
        }

        public static void InitializeSiteArea(string key, Action<bool> initializeCallback)
        {
            Guard.ArgumentNotNullOrEmpty(key, "key");

            lock (_lock)
            {
                if (!_isLoaded)
                {
                    var _token = (IDisposable)null;
                    _token = ObservableExtensions.Subscribe(Channel<SiteMap>.Public, (s) =>
                    {
                        InitializeSiteArea(key, initializeCallback);
                        if (_token != null) _token.Dispose();
                    }, (ex) =>
                    {
                        if (initializeCallback != null) initializeCallback(false);
                        if (_token != null) _token.Dispose();
                    });
                    return;
                }

                if (!_siteMap.Areas.Contains(key))
                {
                    if (initializeCallback != null) initializeCallback(false);
                    return;
                }

                var _area = SiteMap.Areas[key];
                if (_area.InternalIsInitialized)
                {
                    if (initializeCallback != null) initializeCallback(true);
                }
                else
                {
                    LoadSiteArea(_area, initializeCallback);
                }
            }
        }

        public static SiteArea ResolveSiteArea(string key)
        {
            Guard.ArgumentNotNullOrEmpty(key, "key");

            lock (_lock)
            {
                return (SiteMap != null && SiteMap.Areas != null && SiteMap.Areas.Contains(key)) ? SiteMap.Areas[key] : null;
            }
        }

        public static SiteMapNode ResolveSiteMapNode(string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace("key", "key");

            lock (_lock)
            {
                if (SiteMap == null || SiteMap.RootNode == null) return null;
                var _siteMapNode = (SiteMapNode)null;
                return ResolveKeyedSiteMapNode(SiteMap.RootNode, key, out _siteMapNode) ? _siteMapNode : null;
            }
        }

        public static bool IsSiteMapLoaded
        {
            get
            {
                lock (_lock)
                {
                    return _isLoaded;
                }
            }
        }

        public static void LoadSiteMap(Action<SiteMap> loadCallback)
        {
            Guard.ArgumentNotNull(loadCallback, "loadCallback");

            lock (_lock)
            {
                if (_isLoaded)
                {
                    loadCallback(_siteMap);
                }
                else
                {
                    var _token = (IDisposable)null;
                    _token = ObservableExtensions.Subscribe(Channel<SiteMap>.Public, (s) =>
                    {
                        loadCallback(s);
                        if (_token != null) _token.Dispose();
                    }, (ex) =>
                    {
                        loadCallback(null);
                        if (_token != null) _token.Dispose();
                    });
                }
            }
        }

        #endregion

        #region Properties

        public static SiteMap SiteMap
        {
            get
            {
                lock (_lock)
                {
                    return _siteMap;
                }
            }
        }

        #endregion

        #region Helpers

        private static void InitializeOnLoadSiteAreas()
        {
            foreach (var _area in SiteMap.Areas)
            {
                if (_area.InitializeOnLoad)
                {
                    LoadSiteArea(_area, null);
                }
            }
        }

        private static void LoadSiteArea(SiteArea siteArea, Action<bool> loadCallback)
        {
            var _dependentAreas = ResolveFlattenedDependentSiteAreas(siteArea);
            var _orderedDependnetAreas = new Stack<SiteArea>(TopologicallyOrderDependentSiteAreas(_dependentAreas));
            var _subscribeHandler = (Action<LoadedResourceInfo>)null;
            _subscribeHandler = new Action<LoadedResourceInfo>((r) =>
            {
                if (r.IsLoaded)
                {
                    if (_orderedDependnetAreas.Count == 0)
                    {
                        siteArea.InternalIsInitialized = true;
                        if (loadCallback != null) loadCallback(true);
                    }
                    else
                    {
                        var _nextArea = _orderedDependnetAreas.Pop();
                        ObservableExtensions.Subscribe(RemoteResourceLoader.LoadResource(new Uri(_nextArea.RemoteUrl, UriKind.RelativeOrAbsolute)), _subscribeHandler);
                    }
                }
                else
                {
                    if (loadCallback != null) loadCallback(false);
                    loadCallback = null;
                }
            });

            // and we start the chain
            var _area = _orderedDependnetAreas.Pop();
            ObservableExtensions.Subscribe(RemoteResourceLoader.LoadResource(new Uri(_area.RemoteUrl, UriKind.RelativeOrAbsolute)), _subscribeHandler);
        }

        private static IEnumerable<SiteArea> TopologicallyOrderDependentSiteAreas(IEnumerable<SiteArea> areas)
        {
            return DependencyGraph.ResolveTopologicalOrder(
                areas,
                (a) => a.Key,
                (a) => a.HasDependencies ? a.AreaDependencies.Select((ad) => ad.Key) : null);
        }

        private static IEnumerable<SiteArea> ResolveFlattenedDependentSiteAreas(SiteArea siteArea)
        {
            // this aggregrates the dependencies etc into one list
            var _aggregrator = (Action<List<SiteArea>, SiteArea>)null;
            _aggregrator = new Action<List<SiteArea>, SiteArea>((l, a) =>
            {
                if (!l.Contains(a)) l.Add(a);
                if (!a.HasDependencies) return;
                foreach (var _dependnecy in a.AreaDependencies)
                {
                    var _area = SiteMap.Areas[_dependnecy.Key];
                    if (!l.Contains(_area)) _aggregrator(l, _area);
                }
            });

            // we aggregrate all the child areas, recursively 
            var _areas = new List<SiteArea>();
            _aggregrator(_areas, siteArea);
            return _areas;
        }

        private static bool ResolveKeyedSiteMapNode(SiteMapNode siteMapNode, string key, out SiteMapNode keyedSiteMapNode)
        {
            if (string.Equals(siteMapNode.Key, key, StringComparison.InvariantCultureIgnoreCase))
            {
                keyedSiteMapNode = siteMapNode;
                return true;
            }

            if (siteMapNode.HasChildNodes)
            {
                foreach (var _childNode in siteMapNode.ChildNodes)
                {
                    if (ResolveKeyedSiteMapNode(_childNode, key, out keyedSiteMapNode)) return true;
                }
            }

            keyedSiteMapNode = null;
            return false;
        }

        #endregion

    }
}
