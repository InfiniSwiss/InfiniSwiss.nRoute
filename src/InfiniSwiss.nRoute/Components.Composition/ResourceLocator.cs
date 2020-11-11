using System;
using System.Collections.Generic;

namespace nRoute.Components.Composition
{
    public static class ResourceLocator
    {

        #region Static Methods

        // Get Resource Related

        public static T GetResource<T>()
            where
                T : class
        {
            return (T)Resource.GetResource(typeof(T));
        }

        public static object GetResource(Type resourceType)
        {
            return Resource.GetResource(resourceType);
        }

        public static T GetResource<T>(string name)
            where
                T : class
        {
            return (T)Resource.GetResource(typeof(T), name);
        }

        public static object GetResource(Type resourceType, string name)
        {
            return Resource.GetResource(resourceType, name);
        }

        // Try Get Resource Related

        public static bool TryGetResource<T>(out T resourceInstance)
            where
                T : class
        {
            Object _resource = null;
            var _response = Resource.TryGetResource(typeof(T), out _resource);
            resourceInstance = _resource == null ? null : (T)_resource;
            return _response;
        }

        public static bool TryGetResource(Type resourceType, out object resourceInstance)
        {
            return Resource.TryGetResource(resourceType, out resourceInstance);
        }

        public static bool TryGetResource<T>(string name, out T resourceInstance)
            where
                T : class
        {
            Object _resource = null;
            var _response = Resource.TryGetResource(typeof(T), name, out _resource);
            resourceInstance = _resource == null ? null : (T)_resource;
            return _response;
        }

        public static bool TryGetResource(Type resourceType, string name, out object resourceInstance)
        {
            return Resource.TryGetResource(resourceType, name, out resourceInstance);
        }

        // Get Resource Locator Related

        public static IResourceLocator GetResourceLocator<T>()
            where
                T : class
        {
            return Resource.GetResourceLocator(typeof(T));
        }

        public static IResourceLocator GetResourceLocator(Type resourceType)
        {
            return Resource.GetResourceLocator(resourceType);
        }

        public static IResourceLocator GetResourceLocator<T>(string name)
            where
                T : class
        {
            return Resource.GetResourceLocator(typeof(T), name);
        }

        public static IResourceLocator GetResourceLocator(Type resourceType, string name)
        {
            return Resource.GetResourceLocator(resourceType, name);
        }

        // Try Get Resource Locator

        public static bool TryGetResourceLocator<T>(out IResourceLocator resourceLocator)
            where
                T : class
        {
            IResourceLocator _locator = null;
            var _response = Resource.TryGetResourceLocator(typeof(T), out _locator);
            resourceLocator = _locator;
            return _response;
        }

        public static bool TryGetResourceLocator(Type resourceType, out IResourceLocator resourceLocator)
        {
            return Resource.TryGetResourceLocator(resourceType, out resourceLocator);
        }

        public static bool TryGetResourceLocator<T>(string name, out IResourceLocator resourceLocator)
            where
                T : class
        {
            IResourceLocator _locator = null;
            var _response = Resource.TryGetResourceLocator(typeof(T), name, out _locator);
            resourceLocator = _locator;
            return _response;
        }

        public static bool TryGetResourceLocator(Type resourceType, string name,
            out IResourceLocator resourceLocator)
        {
            return Resource.TryGetResourceLocator(resourceType, name, out resourceLocator);
        }

        // Registration Related

        public static bool IsResourceRegistered<T>()
        {
            return IsResourceRegistered(typeof(T));
        }

        public static bool IsResourceRegistered<T>(bool ensureHasDefaultResource)
        {
            return IsResourceRegistered(typeof(T), ensureHasDefaultResource);
        }

        public static bool IsResourceRegistered(Type resourceType)
        {
            return Resource.IsResourceRegistered(resourceType);
        }

        public static bool IsResourceRegistered(Type resourceType, bool ensureHasDefaultResource)
        {
            return Resource.IsResourceRegistered(resourceType, ensureHasDefaultResource);
        }

        public static bool IsResourceRegistered<T>(string name)
        {
            return IsResourceRegistered(typeof(T), name);
        }

        public static bool IsResourceRegistered(Type resourceType, string name)
        {
            return Resource.IsResourceRegistered(resourceType, name);
        }

        public static void SetDefaultResourceLocator<T>(string name)
        {
            SetDefaultResourceLocator(typeof(T), name);
        }

        public static void SetDefaultResourceLocator(Type resourceType, string name)
        {
            Resource.SetDefaultResourceLocator(resourceType, name);
        }

        // Adapter Related

        public static bool IsResourceAdapterAvailable(Type resourceType)
        {
            return Resource.IsResourceAdapterRegistered(resourceType);
        }

        public static bool IsResourceOrAdapterAvailable(Type resourceType)
        {
            return Resource.IsResourceOrAdapterRegistered(resourceType);
        }

        public static bool IsResourceOrAdapterAvailable(Type resourceType, string name)
        {
            return Resource.IsResourceOrAdapterRegistered(resourceType, name);
        }

        // Resource Catalog

        public static IEnumerable<IResourceLocator> GetResourceCatalog<T>()
        {
            return Resource.GetResourceCatalog(typeof(T));
        }

        public static IEnumerable<IResourceLocator> GetResourceCatalog(Type resourceType)
        {
            return Resource.GetResourceCatalog(resourceType);
        }

        #endregion

    }
}
