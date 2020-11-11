using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Services
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MapServiceAttribute
         : MapResourceBaseAttribute
    {
        private const string SERVICE_MUSTBE_OFTYPE = "{0} type must be an implementation of {1}.";

        private readonly string _name;
        public Type ServiceType { get; set; }
        public Type[] Dependencies { get; set; }

        public MapServiceAttribute(Type serviceType)
         : this(serviceType, (string)null, null) { }

        public MapServiceAttribute(Type serviceType, params Type[] dependencies)
         : this(serviceType, null, dependencies) { }

        public MapServiceAttribute(Type serviceType, string name, params Type[] dependencies)
        {
            Guard.ArgumentNotNull(serviceType, "serviceType");
            ServiceType = serviceType;
            _name = name;
            Dependencies = dependencies;
        }

        #region Properties

        public bool IsDefault { get; set; }

        public InstanceLifetime Lifetime { get; set; }

        public InitializationMode InitializationMode { get; set; }

        #endregion

        #region Overrides

        protected internal override bool IsDefaultResource
        {
            get { return IsDefault; }
        }

        protected internal override bool CanInitialize(Type targetType)
        {

            if (Dependencies == null || Dependencies.Length == 0) return true;

            // else we need to check if each of the dependency is available
            foreach (var _dependency in Dependencies)
            {
                if (!ServiceLocator.IsServiceRegistered(_dependency)) return false;
            }
            return true;

        }

        protected internal override Type GetResourceType(Type targetType)
        {
            return ServiceType;
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {

            EnsureIsServiceType(targetType, ServiceType);

            // we create the default resolver
            return new DefaultServicesLocator(new ServiceMeta(targetType, _name, InitializationMode, Lifetime));
        }

        #endregion

        #region Helper

        protected static void EnsureIsServiceType(Type implementationType, Type serviceType)
        {
            if (!serviceType.IsAssignableFrom(implementationType)) throw new InvalidOperationException(
               string.Format(SERVICE_MUSTBE_OFTYPE, implementationType.FullName, serviceType.FullName));
        }

        #endregion

    }
}
