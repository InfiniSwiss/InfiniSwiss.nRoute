using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Services
{
    public class ServiceMeta
    {
        private readonly Type _implementationType;
        private readonly string _serviceName;
        private readonly InitializationMode _initializationMode;
        private readonly InstanceLifetime _lifetime;

        public ServiceMeta(Type implementationType, string serviceName, InitializationMode initializationMode, InstanceLifetime lifetime)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");

            _implementationType = implementationType;
            _serviceName = (!string.IsNullOrEmpty(serviceName)) ? serviceName : _implementationType.FullName;
            _initializationMode = initializationMode;
            _lifetime = lifetime;
        }

        #region Properties

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public Type ImplementationType
        {
            get { return _implementationType; }
        }

        public InitializationMode InitializationMode
        {
            get { return _initializationMode; }
        }

        public InstanceLifetime Lifetime
        {
            get { return _lifetime; }
        }

        #endregion

    }
}
