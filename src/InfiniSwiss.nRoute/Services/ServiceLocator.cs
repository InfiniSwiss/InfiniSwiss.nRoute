using nRoute.Components.Composition;
using System;

namespace nRoute.Services
{
    public static class ServiceLocator
    {

        #region Static Methods

        // Get Service Related

        public static T GetService<T>()
            where
                T : class
        {
            return ResourceLocator.GetResource<T>();
        }

        public static object GetService(Type serviceType)
        {
            return ResourceLocator.GetResource(serviceType);
        }

        public static T GetService<T>(string name)
            where
                T : class
        {
            return ResourceLocator.GetResource<T>(name);
        }

        public static object GetService(Type serviceType, string name)
        {
            return ResourceLocator.GetResource(serviceType, name);
        }

        // Try Get Service Related

        public static bool TryGetService<T>(out T serviceInstance)
            where
                T : class
        {
            return ResourceLocator.TryGetResource<T>(out serviceInstance);
        }

        public static bool TryGetService(Type serviceType, out object serviceInstance)
        {
            return ResourceLocator.TryGetResource(serviceType, out serviceInstance);
        }

        public static bool TryGetService<T>(string name, out T serviceInstance)
            where
                T : class
        {
            return ResourceLocator.TryGetResource<T>(name, out serviceInstance);
        }

        public static bool GetService(Type serviceType, string name, out object serviceInstance)
        {
            return ResourceLocator.TryGetResource(serviceType, name, out serviceInstance);
        }

        // Registration Related

        public static bool IsServiceRegistered<T>()
        {
            return ResourceLocator.IsResourceRegistered(typeof(T));
        }

        public static bool IsServiceRegistered(Type serviceType)
        {
            return ResourceLocator.IsResourceRegistered(serviceType);
        }

        public static bool IsServiceRegistered<T>(string name)
        {
            return ResourceLocator.IsResourceRegistered(typeof(T), name);
        }

        public static bool IsServiceRegistered(Type serviceType, string name)
        {
            return ResourceLocator.IsResourceRegistered(serviceType, name);
        }

        public static void SetDefaultService<T>(string name)
        {
            ResourceLocator.SetDefaultResourceLocator(typeof(T), name);
        }

        public static void SetDefaultService(Type serviceType, string name)
        {
            ResourceLocator.SetDefaultResourceLocator(serviceType, name);
        }

        #endregion

    }
}