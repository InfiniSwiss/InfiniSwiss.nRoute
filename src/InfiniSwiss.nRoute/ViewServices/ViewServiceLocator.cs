using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewServices
{
    public static class ViewServiceLocator
    {
        private const string VIEWSERVICE_NOTFOUND = "ViewService of type '{0}' with name '{1}' not found.";
        private const string INSTANCEREGISTER_ERROR =
            "Instance register/unregistering can only be set with ViewServices that have 'SelfRegisteredInstance' Lifetime ('{0}' does not).";

        #region Static Methods

        // Get ViewService Related

        public static T GetViewService<T>()
            where
                T : class
        {
            return ResourceLocator.GetResource<T>();
        }

        public static object GetViewService(Type viewServiceType)
        {
            return ResourceLocator.GetResource(viewServiceType);
        }

        public static T GetViewService<T>(string name)
            where
                T : class
        {
            return ResourceLocator.GetResource<T>(name);
        }

        public static object GetViewService(Type viewServiceType, string name)
        {
            return ResourceLocator.GetResource(viewServiceType, name);
        }

        // Try Get ViewService Related

        public static bool TryGetViewService<T>(out T viewServiceInstance)
            where
                T : class
        {
            return ResourceLocator.TryGetResource<T>(out viewServiceInstance);
        }

        public static bool TryGetViewService(Type viewServiceType, out object viewServiceInstance)
        {
            return ResourceLocator.TryGetResource(viewServiceType, out viewServiceInstance);
        }

        public static bool TryGetViewService<T>(string name, out T viewServiceInstance)
            where
                T : class
        {
            return ResourceLocator.TryGetResource<T>(name, out viewServiceInstance);
        }

        public static bool GetViewService(Type viewServiceType, string name, out object viewServiceInstance)
        {
            return Resource.TryGetResource(viewServiceType, name, out viewServiceInstance);
        }

        // Registration Related

        public static bool IsServiceRegistered<T>()
        {
            return ResourceLocator.IsResourceRegistered<T>();
        }

        public static bool IsServiceRegistered(Type viewServiceType)
        {
            return ResourceLocator.IsResourceRegistered(viewServiceType, false);
        }

        public static bool IsServiceRegistered(Type viewServiceType, bool ensureHasDefaultService)
        {
            return ResourceLocator.IsResourceRegistered(viewServiceType, ensureHasDefaultService);
        }

        public static bool IsServiceRegistered<T>(string name)
        {
            return ResourceLocator.IsResourceRegistered<T>(name);
        }

        public static bool IsServiceRegistered(Type viewServiceType, string name)
        {
            return ResourceLocator.IsResourceRegistered(viewServiceType, name);
        }

        public static void SetDefaultViewService<T>(string name)
        {
            ResourceLocator.SetDefaultResourceLocator<T>(name);
        }

        public static void SetDefaultViewService(Type viewServiceType, string name)
        {
            ResourceLocator.SetDefaultResourceLocator(viewServiceType, name);
        }

        #endregion

        #region ViewService Instance Related

        public static void RegisterViewServiceInstance<T>(string name, T instance)
        {
            RegisterViewServiceInstance(typeof(T), name, instance);
        }

        public static void RegisterViewServiceInstance(Type viewServiceType, string name, object instance)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            Guard.ArgumentNotNull(instance, "instance");

            // we get the view service locator and we unregister
            var _viewServiceMeta = GetViewServiceMeta(viewServiceType, name);
            if (_viewServiceMeta.Lifetime != ViewServiceLifetime.SelfRegisteredInstance)
                throw new InvalidOperationException(string.Format(INSTANCEREGISTER_ERROR, viewServiceType.FullName));

            // and register
            ViewServicesInstancesManager.RegisterInstance(viewServiceType, instance);
        }

        public static void UnregisterViewServiceInstance<T>(string name, T instance)
        {
            UnregisterViewServiceInstance(typeof(T), name, instance);
        }

        public static void UnregisterViewServiceInstance(Type viewServiceType, string name,
            object instance)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            Guard.ArgumentNotNull(instance, "instance");

            // we get the view service locator and we unregister
            var _viewServiceMeta = GetViewServiceMeta(viewServiceType, name);
            if (_viewServiceMeta.Lifetime != ViewServiceLifetime.SelfRegisteredInstance)
                throw new InvalidOperationException(string.Format(INSTANCEREGISTER_ERROR, viewServiceType.FullName));

            // and unregister
            ViewServicesInstancesManager.UnregisterInstance(viewServiceType, instance);
        }

        #endregion

        #region Helpers

        private static ViewServiceMeta GetViewServiceMeta(Type viewServiceType, string name)
        {
            Guard.ArgumentNotNull(viewServiceType, "viewServiceType");
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            // first we get the view service locator
            IResourceLocator _locator;
            if (!ResourceLocator.TryGetResourceLocator(viewServiceType, name, out _locator))
            {
                throw new InvalidOperationException(string.Format(VIEWSERVICE_NOTFOUND,
                    viewServiceType.FullName, name));
            }

            // and return
            return (ViewServiceMeta)_locator.ResourceMeta;
        }

        #endregion

    }
}
