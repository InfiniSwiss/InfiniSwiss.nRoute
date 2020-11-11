using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class MapViewServiceAttribute
         : MapResourceBaseAttribute
    {
        private const string MUSTBE_VIEWSERVICE_TYPE = "{0} type must be an implementation of {1}.";

        public Type ViewServiceType { get; set; }
        public Type[] Dependencies { get; set; }

        private readonly string _name;

        public MapViewServiceAttribute(Type viewServiceType, params Type[] dependencies)
            : this(viewServiceType, null, dependencies) { }

        public MapViewServiceAttribute(Type viewServiceType, string name, params Type[] dependencies)
        {
            Guard.ArgumentNotNull(viewServiceType, "viewServiceType");

            // save
            ViewServiceType = viewServiceType;
            Dependencies = dependencies;
            _name = name;
            Lifetime = ViewServiceLifetime.Singleton;
            InitializationMode = InitializationMode.OnDemand;
        }

        #region Properties

        public ViewServiceLifetime Lifetime { get; set; }

        public InitializationMode InitializationMode { get; set; }

        public bool IsDefault { get; set; }

        #endregion

        #region Overrides

        protected internal override bool IsDefaultResource
        {
            get { return IsDefault; }
        }

        protected internal override bool CanInitialize(Type targetType)
        {
            return true;
        }

        protected internal override Type GetResourceType(Type targetType)
        {
            return ViewServiceType;
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            EnsureIsViewServiceType(targetType, ViewServiceType);
            return new DefaultViewServiceLocator(new ViewServiceMeta(ViewServiceType, targetType, _name,
                InitializationMode, Lifetime));
        }

        #endregion

        #region Helpers

        protected static void EnsureIsViewServiceType(Type implementationType, Type viewServiceType)
        {
            if (!viewServiceType.IsAssignableFrom(implementationType)) throw new InvalidOperationException(
                string.Format(MUSTBE_VIEWSERVICE_TYPE, implementationType.FullName, viewServiceType.FullName));
        }

        #endregion

    }
}
