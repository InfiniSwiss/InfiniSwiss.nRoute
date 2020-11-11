using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class MapViewAttribute
         : MapViewViewModelBaseAttribute
    {
        public MapViewAttribute(Type viewModelType)
        {
            Guard.ArgumentNotNull(viewModelType, "viewModelType");
            this.ViewModelType = viewModelType;
        }

        public Type ViewModelType { get; private set; }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return new DefaultViewModelLocator(new ViewModelMeta(targetType, ViewModelType));
        }

        #endregion

    }
}