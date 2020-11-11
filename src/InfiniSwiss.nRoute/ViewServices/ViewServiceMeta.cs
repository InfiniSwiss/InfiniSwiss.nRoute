using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewServices
{
    public class ViewServiceMeta
    {
        private readonly Type _viewServiceType;
        private readonly Type _implementationType;
        private readonly string _viewServiceName;
        private readonly ViewServiceLifetime _lifetime;
        private readonly InitializationMode _initializationMode;

        public ViewServiceMeta(Type viewServiceType, Type implementationType,
            string viewServiceName, InitializationMode initializationMode, ViewServiceLifetime lifetime)
        {
            Guard.ArgumentNotNull(viewServiceType, "viewServiceType");
            Guard.ArgumentNotNull(implementationType, "implementationType");
            if (string.IsNullOrEmpty(viewServiceName)) viewServiceName = implementationType.FullName;

            // we save 
            _viewServiceType = viewServiceType;
            _implementationType = implementationType;
            _viewServiceName = viewServiceName;
            _lifetime = lifetime;
            _initializationMode = initializationMode;
        }

        #region Properties

        public string ViewServiceName
        {
            get { return _viewServiceName; }
        }

        public Type ViewServiceType
        {
            get { return _viewServiceType; }
        }

        public Type ImplementationType
        {
            get { return _implementationType; }
        }

        public ViewServiceLifetime Lifetime
        {
            get { return _lifetime; }
        }

        public InitializationMode InitializationMode
        {
            get { return _initializationMode; }
        }

        #endregion

    }
}
