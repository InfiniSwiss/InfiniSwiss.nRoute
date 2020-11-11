using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineViewServiceAttribute
         : MapViewServiceAttribute
    {
        public Type ImplementationType { get; set; }

        public DefineViewServiceAttribute(Type viewServiceType, Type implementationType)
         : this(viewServiceType, implementationType, null) { }

        public DefineViewServiceAttribute(Type viewServiceType, Type implementationType, string name,
            params Type[] dependencies)
         : base(viewServiceType, name, dependencies)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            EnsureIsViewServiceType(implementationType, viewServiceType);

            // and we save the reference
            ImplementationType = implementationType;
        }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            // we overide the target type
            return base.GetResourceLocator(ImplementationType);
        }

        #endregion

    }
}
