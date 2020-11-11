using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineServiceAttribute
         : MapServiceAttribute
    {
        public Type ImplementationType { get; set; }

        public DefineServiceAttribute(Type serviceType, Type implementationType, params Type[] dependencies)
         : this(serviceType, implementationType, null, dependencies) { }

        public DefineServiceAttribute(Type serviceType, Type implementationType, string name, params Type[] dependencies)
         : base(serviceType, name, dependencies)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            EnsureIsServiceType(implementationType, serviceType);

            // save this
            this.ImplementationType = implementationType;
        }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return base.GetResourceLocator(ImplementationType);
        }

        #endregion

    }
}
