using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class MapViewModelAttribute
         : MapViewViewModelBaseAttribute
    {
        public MapViewModelAttribute(Type viewType)
        {
            Guard.ArgumentNotNull(viewType, "viewType");
            this.ViewType = viewType;
        }

        public Type ViewType { get; private set; }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return new DefaultViewModelLocator(new ViewModelMeta(ViewType, targetType));
        }

        #endregion

    }
}