using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineViewViewModelAttribute
         : MapViewViewModelBaseAttribute
    {
        public DefineViewViewModelAttribute(Type viewType, Type viewModelType)
        {
            Guard.ArgumentNotNull(viewType, "viewType");
            Guard.ArgumentNotNull(viewModelType, "viewModelType");

            this.ViewType = viewType;
            this.ViewModelType = viewModelType;
        }

        public Type ViewType { get; private set; }

        public Type ViewModelType { get; private set; }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return new DefaultViewModelLocator(new ViewModelMeta(ViewType, ViewModelType));
        }

        #endregion

    }
}