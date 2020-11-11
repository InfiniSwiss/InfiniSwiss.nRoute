using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace nRoute.Components.Routing
{
    /// <summary>
    /// Provides a collection of routes for routing. 
    /// </summary>
    public class RouteCollection
         : Collection<RouteBase>
    {

        #region Constants

        private const string ROUTING_URL_REQUIRED = "Ravigation Url must be specified.";
        private const string ROUTE_COLLECTION_DUPLICATE =
            "The route provided already exists in the route collection. The collection may not contain duplicate routes.";
        private const string ROUTE_COLLECTION_DUPLICATE_NAME =
            "A route named '{0}' is already in the route collection. Route names must be unique.";
        private const string ROUTE_COLLECTION_REQUIRES_CONTEXT =
            "request.Current must be non-null when a IRoutingContext is not provided.";
        private const string ROUTE_COLLECTION_CONTEXT_MISSING = "The context does not contain any request data.";
        private const string ROUTE_COLLECTION_NAME_NOT_FOUND = "A route named '{0}' could not be found in the route collection.";
        private const string ROUTE_INVALID_ROUTE_URL =
            "The routing url cannot start with a '/' or '~' character and it cannot contain a '?' character.";

        #endregion

        private Dictionary<string, RouteBase> _namedMap;
        private ReaderWriterLock _readerWriterLock;

        public RouteCollection()
        {
            this._namedMap = new Dictionary<string, RouteBase>(StringComparer.OrdinalIgnoreCase);
            this._readerWriterLock = new ReaderWriterLock();
        }

        public void Add(string name, RouteBase item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentValue((!string.IsNullOrEmpty(name) && this._namedMap.ContainsKey(name)), "name",
                ROUTE_COLLECTION_DUPLICATE_NAME, name);

            base.Add(item);
            if (!string.IsNullOrEmpty(name))
            {
                this._namedMap[name] = item;
            }
        }

        protected override void ClearItems()
        {
            this._namedMap.Clear();
            base.ClearItems();
        }

        public IDisposable GetReadLock()
        {
            //this._readerWriterLock.AcquireReaderLock();
            //this._readerWriterLock.AcquireReaderLock(Timeout.Infinite);
            var _lock = new ReadLockDisposable(this._readerWriterLock);
            return _lock.AcquireReaderLock();
        }

        public IDisposable GetWriteLock()
        {
            //this._readerWriterLock.AcquireWriterLock(Timeout.Infinite);
            //return new WriteLockDisposable(this._readerWriterLock);
            var _lock = new WriteLockDisposable(this._readerWriterLock);
            return _lock.AcquireWriterLock();
        }

        //private IRoutingContext GetContext(IRoutingContext context)
        //{
        //    if (context != null)
        //    {
        //        return context;
        //    }
        //    IRoutingRequest current = context.IRoutingRequest;
        //    if (current == null)
        //    {
        //        throw new InvalidOperationException(RouteCollection_RequiresContext);
        //    }
        //    return new RoutingContext(current, new RouteData());
        //}

        public RouteData GetRouteData(IUrlRequest request)
        {
            Guard.ArgumentNotNull(request, "request");
            Guard.ArgumentValue(string.IsNullOrEmpty(request.RequestUrl), "request", ROUTING_URL_REQUIRED);
            Guard.ArgumentValue(((request.RequestUrl.StartsWith("~", StringComparison.Ordinal) ||
                request.RequestUrl.StartsWith("/", StringComparison.Ordinal)) ||
                (request.RequestUrl.IndexOf('?') != -1)), "url", ROUTE_INVALID_ROUTE_URL);

            using (this.GetReadLock())
            {
                foreach (RouteBase base2 in this)
                {
                    RouteData routeData = base2.GetRouteData(request);
                    if (routeData != null)
                    {
                        return routeData;
                    }
                }
            }
            return null;
        }

        private static string GetUrlWithApplicationPath(IRoutingContext context, string url)
        {
            string str = url ?? string.Empty;
            if (!str.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                str = str + "/";
            }
            return str;
        }

        ///// <summary>Their are not virtual or full paths, we only have relative paths</summary>
        //public VirtualPathData GetVirtualPath(IRoutingContext context, ParametersDictionary values)
        //{
        //    using (this.GetReadLock())
        //    {
        //        foreach (RouteBase base2 in this)
        //        {
        //            VirtualPathData virtualPath = base2.GetVirtualPath(context, values);
        //            if (virtualPath != null)
        //            {
        //                virtualPath.VirtualPath = GetUrlWithApplicationPath(context, virtualPath.VirtualPath);
        //                return virtualPath;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public VirtualPathData GetVirtualPath(IRoutingContext context, string name, ParametersDictionary values)
        //{
        //    RouteBase base2;
        //    bool flag;
        //    if (string.IsNullOrEmpty(name))
        //    {
        //        return this.GetVirtualPath(context, values);
        //    }
        //    using (this.GetReadLock())
        //    {
        //        flag = this._namedMap.TryGetValue(name, out base2);
        //    }
        //    if (!flag)
        //    {
        //        throw new ArgumentException(string.Format(RouteCollection_NameNotFound, new object[] { name }), "name");
        //    }
        //    VirtualPathData virtualPath = base2.GetVirtualPath(context, values);
        //    if (virtualPath == null)
        //    {
        //        return null;
        //    }
        //    virtualPath.VirtualPath = GetUrlWithApplicationPath(context, virtualPath.VirtualPath);
        //    return virtualPath;
        //}

        protected override void InsertItem(int index, RouteBase item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentValue(base.Contains(item), "item", ROUTE_COLLECTION_DUPLICATE);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this.RemoveRouteName(index);
            base.RemoveItem(index);
        }

        private void RemoveRouteName(int index)
        {
            RouteBase base2 = base[index];
            foreach (KeyValuePair<string, RouteBase> pair in this._namedMap)
            {
                if (pair.Value == base2)
                {
                    this._namedMap.Remove(pair.Key);
                    break;
                }
            }
        }

        protected override void SetItem(int index, RouteBase item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentValue(base.Contains(item), "item", ROUTE_COLLECTION_DUPLICATE);

            this.RemoveRouteName(index);
            base.SetItem(index, item);
        }

        public RouteBase this[string name]
        {
            get
            {
                RouteBase base2;
                if (!string.IsNullOrEmpty(name) && this._namedMap.TryGetValue(name, out base2))
                {
                    return base2;
                }
                return null;
            }
        }

    }

}
