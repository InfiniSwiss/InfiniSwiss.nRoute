using nRoute.Components.Composition.Adapters;
using nRoute.Components.Mapping;
using nRoute.Components.Messaging;
using nRoute.Internal;
using System;
using System.Collections.Generic;

namespace nRoute.Components.Composition
{
    public abstract class Resource
    {

        #region Declarations

        private const string CANNOT_RESOLVE_RESOURCE = "Cannot resolve resource of type {0}";
        private const string CANNOT_RESOLVE_LOCATOR = "Cannot resolve locator for type {0}";
        private const string CANNOT_RESOLVE_ON = "Could not resolve resource type on {0}";
        private const string CANNOT_RESOLVE_IRESOURCELOCATOR = "Could not resolve IResourceLocator on {0}";

        private readonly static Dictionary<Type, Resource> _resourcesCatalogs;
        private readonly static List<KeyValuePair<Func<Type, bool>, Func<Type, ILocatorAdapter>>> _resourceAdapters;
        private readonly static List<PendingMapping> _pendingMappings;
        private readonly static ChannelObserver<MappedAssemblyInfo> _assemblyObserver;
        private readonly static Type _invokerGenericType = typeof(ResourceInvoker<Object>).GetGenericTypeDefinition();
        private readonly static Object _lock = new Object();

        #endregion

        static Resource()
        {
            _resourcesCatalogs = new Dictionary<Type, Resource>();
            _resourceAdapters = new List<KeyValuePair<Func<Type, bool>, Func<Type, ILocatorAdapter>>>();
            _pendingMappings = new List<PendingMapping>();
            _assemblyObserver = new ChannelObserver<MappedAssemblyInfo>(OnAssemblyMapped);

            // and register built-in adapters
            KnownAdapters.RegisterAdapters();
        }

        internal Resource(Type resourceType)
        {
            lock (_lock)
            {
                _resourcesCatalogs.Add(resourceType, this);
            }
        }

        #region Resources Static Methods

        // Resource Related

        protected internal static object GetResource(Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                // first we check our catalog
                if (_resourcesCatalogs.ContainsKey(resourceType))
                {
                    return _resourcesCatalogs[resourceType].ResolveResourceInstance(false);
                }

                // next we check our adapters
                var _adapter = ResolveApplicableAdapter(resourceType);
                if (_adapter != null)
                {
                    return _adapter(resourceType).Resolve(null);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CANNOT_RESOLVE_RESOURCE, resourceType.FullName));
                }
            }
        }

        protected internal static object GetResource(Type resourceType, string name)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                // first we check our catalog
                if (_resourcesCatalogs.ContainsKey(resourceType))
                {
                    return _resourcesCatalogs[resourceType].ResolveResourceInstance(name, true);
                }

                // next we check our adapters
                var _adapter = ResolveApplicableAdapter(resourceType);
                if (_adapter != null)
                {
                    return _adapter(resourceType).Resolve(name);
                }
                else
                {
                    throw new InvalidOperationException(string.Format(CANNOT_RESOLVE_RESOURCE, resourceType.FullName));
                }
            }
        }

        protected internal static bool TryGetResource(Type resourceType, out object resourceInstance)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            // by default
            resourceInstance = null;

            lock (_lock)
            {
                // first we check our catalog
                if (_resourcesCatalogs.ContainsKey(resourceType))
                {
                    resourceInstance = _resourcesCatalogs[resourceType].ResolveResourceInstance(false);
                    if (resourceInstance != null) return true;
                }

                // first we check our adapters
                var _adapter = ResolveApplicableAdapter(resourceType);
                if (_adapter != null)
                {
                    resourceInstance = _adapter(resourceType).Resolve(null);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected internal static bool TryGetResource(Type resourceType, string name, out object resourceInstance)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            // by default
            resourceInstance = null;

            lock (_lock)
            {
                // first we check our catalog
                if (_resourcesCatalogs.ContainsKey(resourceType))
                {
                    resourceInstance = _resourcesCatalogs[resourceType].ResolveResourceInstance(name, false);
                    if (resourceInstance != null) return true;
                }

                // first we check our adapters
                var _adapter = ResolveApplicableAdapter(resourceType);
                if (_adapter != null)
                {
                    resourceInstance = _adapter(resourceType).Resolve(name);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Locator Related

        protected internal static IResourceLocator GetResourceLocator(Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) throw new InvalidOperationException(
                    string.Format(CANNOT_RESOLVE_LOCATOR, resourceType.FullName));
                return _resourcesCatalogs[resourceType].ResolveResourceLocator(false);
            }
        }

        protected internal static IResourceLocator GetResourceLocator(Type resourceType, string name)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) throw new InvalidOperationException(
                    string.Format(CANNOT_RESOLVE_LOCATOR, resourceType.FullName));
                return _resourcesCatalogs[resourceType].ResolveResourceLocator(name, true);
            }
        }

        protected internal static bool TryGetResourceLocator(Type resourceType, out IResourceLocator resourceLocator)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            // by default
            resourceLocator = null;

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) return false;
                resourceLocator = _resourcesCatalogs[resourceType].ResolveResourceLocator(false);
                return (resourceLocator != null);
            }
        }

        protected internal static bool TryGetResourceLocator(Type resourceType, string name, out IResourceLocator resourceLocator)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            // by default
            resourceLocator = null;

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) return false;
                resourceLocator = _resourcesCatalogs[resourceType].ResolveResourceLocator(name, false);
                return (resourceLocator != null);
            }
        }

        // Registration Related

        protected internal static void RegisterAsKnownResourceType(Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType))
                {
                    // this creates the catalog if did not exist
                    var _invoker = (ResourceInvoker)Activator.CreateInstance(_invokerGenericType.MakeGenericType(resourceType));
                    var _catalog = _invoker.GetResourceCatalog();
                    _catalog.IsKnownResource = true;
                }
            }
        }

        protected internal static bool IsResourceRegistered(Type resourceType)
        {
            return IsResourceRegistered(resourceType, true);
        }

        protected internal static bool IsResourceRegistered(Type resourceType, bool ensureHasDefaultResource)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) return false;
                var _catalog = _resourcesCatalogs[resourceType];
                return (ensureHasDefaultResource) ? _catalog.IsAnyResourceRegistered() : _catalog.IsKnownResource;
            }
        }

        protected internal static bool IsResourceRegistered(Type resourceType, string name)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) return false;
                return _resourcesCatalogs[resourceType].IsNamedResourceRegistered(name);
            }
        }

        protected internal static bool IsResourceAdapterRegistered(Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                return (ResolveApplicableAdapter(resourceType) != null);
            }
        }

        protected internal static bool IsResourceOrAdapterRegistered(Type resourceType)
        {
            return IsResourceOrAdapterRegistered(resourceType, null);
        }

        protected internal static bool IsResourceOrAdapterRegistered(Type resourceType, string name)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            if (string.IsNullOrEmpty(name))
            {
                if (IsResourceRegistered(resourceType, true)) return true;
            }
            else
            {
                if (IsResourceRegistered(resourceType, name)) return true;
            }
            return IsResourceAdapterRegistered(resourceType);
        }

        protected internal static void SetDefaultResourceLocator(Type resourceType, string name)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) throw new InvalidOperationException(
                    string.Format(CANNOT_RESOLVE_RESOURCE, resourceType.FullName));
                _resourcesCatalogs[resourceType].SetDefaultResourceLocator(name);
            }
        }

        protected internal static void RegisterResourceLocator(Type resourceType, IResourceLocator locator, bool isDefault)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                // we get the resource catalog
                Resource _resourceCatalog = null;
                if (!_resourcesCatalogs.TryGetValue(resourceType, out _resourceCatalog))
                {
                    // note_ we use activator because the classes are private and not public
                    var _invoker = (ResourceInvoker)Activator.CreateInstance(_invokerGenericType.MakeGenericType(resourceType));
                    _resourceCatalog = _invoker.GetResourceCatalog();
                }

                // we add to the listings
                _resourceCatalog.RegisterResourceLocator(locator, isDefault);
                if (!_resourceCatalog.IsKnownResource) _resourceCatalog.IsKnownResource = true;

            }
        }

        protected internal static void UnregisterResourceLocator(Type resourceType, string name)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            lock (_lock)
            {
                // we get the resource catalog
                Resource _resourceCatalog = null;
                if (!_resourcesCatalogs.TryGetValue(resourceType, out _resourceCatalog))
                {
                    throw new InvalidOperationException("Cannot resolve resource of type "
                        + resourceType.FullName);
                }

                // we add to the listings
                _resourceCatalog.UnregisterResource(name);
            }
        }

        // Catalog Related

        internal static IEnumerable<IResourceLocator> GetResourceCatalog(Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");

            lock (_lock)
            {
                if (!_resourcesCatalogs.ContainsKey(resourceType)) return null;
                return _resourcesCatalogs[resourceType] as IEnumerable<IResourceLocator>;
            }
        }

        #endregion

        #region Public Methods

        // For Unresolved Mapping

        public static void ThrowIfHasUnresolvedMapping()
        {
            lock (_lock)
            {
                if (_pendingMappings.Count > 0)
                {
                    throw new UnresolvedMappingsException(_pendingMappings[0].TargetType,
                        _pendingMappings[0].ResourceAttribute.GetType());
                }
            }
        }

        public static void RegisterLocatorAdapter(Func<Type, bool> adapterPredicate, Func<Type, ILocatorAdapter> adapterFactory)
        {
            Guard.ArgumentNotNull(adapterPredicate, "adapterPredicate");
            Guard.ArgumentNotNull(adapterFactory, "adapterFactory");

            lock (_lock)
            {
                _resourceAdapters.Add(new KeyValuePair<Func<Type, bool>, Func<Type, ILocatorAdapter>>(adapterPredicate, adapterFactory));
            }
        }

        #endregion

        #region Resource Instance Properties

        protected bool IsKnownResource { get; set; }

        #endregion

        #region Resource Instance Methods

        protected abstract Object ResolveResourceInstance(bool throwIfNotFound);

        protected abstract Object ResolveResourceInstance(string name, bool throwIfNotFound);

        protected abstract IResourceLocator ResolveResourceLocator(bool throwIfNotFound);

        protected abstract IResourceLocator ResolveResourceLocator(string name, bool throwIfNotFound);

        protected abstract bool IsAnyResourceRegistered();

        protected abstract bool IsNamedResourceRegistered(string name);

        protected abstract void RegisterResourceLocator(IResourceLocator locator, bool isDefault);

        protected abstract void UnregisterResource(string name);

        protected abstract void SetDefaultResourceLocator(string name);

        #endregion

        #region Assembly Mapping Related

        internal static void Initialize()
        {
            lock (_lock)
            {
                _assemblyObserver.Subscribe(true);
                AssemblyMapper.MapApplicationAssemblies();
            }
        }

        private static void OnAssemblyMapped(MappedAssemblyInfo info)
        {
            if (info == null || info.Assembly == null) return;

            // else we load
            lock (_lock)
            {
                // we first look for given known type of resources
                AssemblyMapper.GetAttributesInMappedAssembly<MapAsKnownResourceAttribute>(info.Assembly, (a, t) =>
                {
                    if (a == null) return;
                    a.RegisterResource(t);
                });

                // we map every attribute - note_, a is the attribute instance and t is type it is applied on
                AssemblyMapper.GetAttributesInMappedAssembly<MapResourceBaseAttribute>(info.Assembly, (a, t) =>
                {
                    // we get the attribute, and use it resolve 
                    if (a == null) return;

                    // we check if all things are resolved
                    if (a.CanInitialize(t))
                    {
                        CatalogResource(a, t);
                    }
                    else
                    {
                        _pendingMappings.Add(new PendingMapping(a, t));
                    }

                });

                // we check if any there are any pending items and as to if they can be resolved
                if (_pendingMappings.Count > 0)
                {

                    bool _isResolving = false;
                    while (true)
                    {
                        var _resourceMappings = _pendingMappings.ToArray();
                        foreach (var _resourceMapping in _resourceMappings)
                        {
                            // if it can 
                            if (_resourceMapping.ResourceAttribute.CanInitialize(_resourceMapping.TargetType))
                            {
                                CatalogResource(_resourceMapping.ResourceAttribute, _resourceMapping.TargetType);
                                _pendingMappings.Remove(_resourceMapping);
                                _isResolving = true;
                            }
                        }

                        // if not even one entity was resolved then we return
                        if (!_isResolving) break;
                        _isResolving = false;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private static void CatalogResource(MapResourceBaseAttribute resourceAttribute, Type targetType)
        {
            // we get the resource type
            var _resourceType = resourceAttribute.GetResourceType(targetType);
            if (_resourceType == null) throw new InvalidOperationException(
                string.Format(CANNOT_RESOLVE_ON, targetType.FullName));

            // we get the locator
            var _resourceLocator = resourceAttribute.GetResourceLocator(targetType);
            if (_resourceLocator == null) throw new InvalidOperationException(
                string.Format(CANNOT_RESOLVE_IRESOURCELOCATOR, targetType.FullName));

            // we get the resource catalog
            RegisterResourceLocator(_resourceType, _resourceLocator, resourceAttribute.IsDefaultResource);
        }

        private static Func<Type, ILocatorAdapter> ResolveApplicableAdapter(Type resourceType)
        {
            foreach (var _kv in _resourceAdapters)
            {
                // if an adapter predicate says yes, then we let it handle it
                if (_kv.Key(resourceType))
                {
                    return _kv.Value;
                }
            }
            return null;
        }

        #endregion

        #region Nested Classes

        private abstract class ResourceInvoker
        {
            public abstract Resource GetResourceCatalog();
        }

        private class ResourceInvoker<T>
            : ResourceInvoker
            where
                T : class
        {
            public override Resource GetResourceCatalog()
            {
                return Resource<T>.Catalog;
            }
        }

        private class PendingMapping
        {
            public PendingMapping(MapResourceBaseAttribute resourceAttribute, Type targetType)
            {
                ResourceAttribute = resourceAttribute;
                TargetType = targetType;
            }

            public MapResourceBaseAttribute ResourceAttribute { get; set; }

            public Type TargetType { get; set; }
        }

        #endregion

    }
}