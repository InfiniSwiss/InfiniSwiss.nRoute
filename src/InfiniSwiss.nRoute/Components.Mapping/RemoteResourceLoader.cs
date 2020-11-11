using nRoute.Components.Messaging;
using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace nRoute.Components.Mapping
{
    public static class RemoteResourceLoader
    {
        private const string DLL_EXT = ".dll";
        private const string XAP_EXT = ".xap";
        private const string DLLXAP_ONLY_MESSAGE = "RemoteResourceLoaded can only download, dll or xap files. '{0}' cannot be loaded";
        private const string DLL_ONLY_MESSAGE = "RemoteResourceLoaded can only download dll files. '{0}' cannot be loaded";

        private readonly static List<Uri> _loadingPackages;
        private readonly static List<Uri> _loadedPackages;
        private readonly static Object _lock = new Object();

        static RemoteResourceLoader()
        {
            _loadedPackages = new List<Uri>();
            _loadingPackages = new List<Uri>();
        }

        #region Main Methods

        public static bool IsResourceLoaded(Uri resourceUri)
        {
            Guard.ArgumentNotNull(resourceUri, "resourceUri");
            lock (_lock)
            {
                return _loadedPackages.Any((u) => IsUriEqual(u, resourceUri));
            }
        }

        public static IObservable<LoadedResourceInfo> LoadResource(Uri resourceUri)
        {
            Guard.ArgumentNotNull(resourceUri, "resourceUri");
            Guard.ArgumentValue((resourceUri.ToString().EndsWith(DLL_EXT, StringComparison.OrdinalIgnoreCase) == false), "resourceUri",
               DLL_ONLY_MESSAGE, resourceUri.ToString());

            return new LazyRelayObservable<LoadedResourceInfo>((o) =>
            {
                lock (_lock)
                {
                    // if already loaded
                    if (_loadedPackages.Any((u) => IsUriEqual(u, resourceUri)))
                    {
                        o.OnNext(new LoadedResourceInfo(resourceUri, true, null));
                        o.OnCompleted();
                        return;
                    }

                    // if loading, we wait for it to be loaded
                    if (_loadingPackages.Any((u) => IsUriEqual(u, resourceUri)))
                    {
                        var _token = (IDisposable)null;
                        _token = ObservableExtensions.Subscribe(Channel<LoadedResourceInfo>.Public, (r) =>
                        {
                            if (IsUriEqual(r.ResourceUri, resourceUri))
                            {
                                o.OnNext(r);
                                o.OnCompleted();
                                _token.Dispose();
                            }
                        });
                        return;
                    }

                    // else add that we are loading
                    _loadingPackages.Add(resourceUri);
                }

                if (resourceUri.IsAbsoluteUri)
                {
                    // and we load
                    var _webClient = new WebClient();
                    _webClient.DownloadDataCompleted += (s, e) =>
                    {
                        var _remoteResourceInfo = new LoadedResourceInfo(resourceUri, (!e.Cancelled && e.Error == null), e.Error);
                        lock (_lock)
                        {
                            _loadingPackages.Remove(resourceUri);

                            if (_remoteResourceInfo.IsLoaded)
                            {
                                _loadedPackages.Add(resourceUri);
                                ProcessAssemblyDownload(e.Result);
                            }
                        }

                        Channel<LoadedResourceInfo>.Public.OnNext(_remoteResourceInfo);
                        o.OnNext(_remoteResourceInfo);
                        o.OnCompleted();
                    };

                    _webClient.DownloadDataAsync(resourceUri);
                }
                else
                {
                    var _remoteResourceInfo = (LoadedResourceInfo)null;
                    try
                    {
                        lock (_lock)
                        {
                            _loadingPackages.Remove(resourceUri);

                            var _fileName = resourceUri.OriginalString;
                            var _filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
                            var _assembly = Assembly.LoadFile(_filePath);
                            _loadedPackages.Add(resourceUri);

                            AssemblyMapper.MapAssembly(_assembly);
                            _remoteResourceInfo = new LoadedResourceInfo(resourceUri, true, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        _remoteResourceInfo = new LoadedResourceInfo(resourceUri, false, ex);
                    }

                    Channel<LoadedResourceInfo>.Public.OnNext(_remoteResourceInfo);
                    o.OnNext(_remoteResourceInfo);
                    o.OnCompleted();
                }

            });
        }

        #endregion

        #region Helpers

        private static void ProcessAssemblyDownload(Byte[] data)
        {
            // and map the assembly
            var _assembly = Assembly.Load(data);
            AssemblyMapper.MapAssembly(_assembly);
        }

        private static bool IsUriEqual(Uri url1, Uri url2)
        {
            if (url1 == null && url2 == null) return true;
            if (url1 == null || url2 == null) return false;
            return (Uri.Compare(url1, url2, UriComponents.HttpRequestUrl, UriFormat.UriEscaped,
                        StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        #endregion

    }
}